// Copyright 2022 王建军
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using NHibernate;
using NHibernate.Type;
using System.Reflection;
using System.Security.Principal;

namespace NHibernateUtils;

public class AuditingInterceptor : EmptyInterceptor
{
    readonly IPrincipal? _principal;

    public AuditingInterceptor(IPrincipal? principal)
    {
        _principal = principal;
    }

    private string? GetCurrentUserName()
    {
        return _principal?.Identity?.Name;
    }
    public override bool OnFlushDirty(object entity, object id, object?[] currentState, object?[] previousState, string[] propertyNames, IType[] types)
    {
        bool modified = false;
        foreach (var prop in entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
            if (prop.IsDefined(typeof(ModificationTimeAttribute), true))
            {
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    if (prop.Name.Equals(propertyNames[i]))
                    {
                        currentState[i] = DateTime.Now;
                        modified = true;
                    }
                }
            }

            if (prop.IsDefined(typeof(ModificationUserAttribute), true))
            {
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    if (prop.Name.Equals(propertyNames[i]))
                    {
                        currentState[i] = GetCurrentUserName();
                        modified = true;
                    }
                }
            }
        }

        return modified;
    }

    public override bool OnSave(object entity, object id, object?[] state, string[] propertyNames, IType[] types)
    {
        bool modified = false;

        foreach (var prop in entity.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
        {
            if (prop.IsDefined(typeof(CreationTimeAttribute), true) || prop.IsDefined(typeof(ModificationTimeAttribute), true))
            {
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    if (prop.Name.Equals(propertyNames[i]))
                    {
                        state[i] = DateTime.Now;
                        modified = true;
                    }
                }
            }

            if (prop.IsDefined(typeof(CreationUserAttribute), true) || prop.IsDefined(typeof(ModificationUserAttribute), true))
            {
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    if (prop.Name.Equals(propertyNames[i]))
                    {
                        state[i] = GetCurrentUserName();
                        modified = true;
                    }
                }
            }
        }

        return modified;
    }
}
