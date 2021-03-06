﻿using System;
using System.ComponentModel;
using System.Reflection;
using KSociety.Base.Domain.Shared.Event;
using Microsoft.Extensions.Logging;

namespace KSociety.Base.Domain.Shared.Class
{
    public class Entity : BaseEntity
    {
        #region [Constructor]


        #endregion

        public void ModifyField(string fieldName, string value)
        {
            Logger?.LogTrace("ModifyField Entity: " +
                             GetType().FullName + "." +
                             MethodBase.GetCurrentMethod()?.Name +
                             " - " + fieldName + " - " + value);

            AddEntityModifyFieldEvent(fieldName, value);
            try
            {
                var field = GetType().GetProperty(fieldName); //, BindingFlags.Public | BindingFlags.Instance);
                if (field != null && field.CanWrite)
                {
                    //Console.WriteLine("1 " + field.Name);
                    var t = Nullable.GetUnderlyingType(field.PropertyType) ?? field.PropertyType;
                    object safeValue = null;

                    //If is byte[]
                    if (t == typeof(byte[]))
                    {
                        if (value != null)
                        {
                            var splitResult = value.Split('-');

                            if (splitResult.Length > 0)
                            {
                                var byteArray = new byte[splitResult.Length];

                                for (var i = 0; i < splitResult.Length; i++)
                                {
                                    byteArray[i] = Convert.ToByte(splitResult[i], 16);
                                }

                                safeValue = byteArray;
                            }
                            else
                            {
                                Logger?.LogWarning("ModifyField Entity: " +
                                                   GetType().FullName + "." +
                                                   MethodBase.GetCurrentMethod()?.Name +
                                                   " - " + fieldName + " - " + value + ": Byte Array on data!");
                            }
                        }
                    }
                    //if (t == typeof(Guid))
                    //{
                    //    ////T t = (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(text);
                    //    //safeValue = (value == null)
                    //    //    ? null
                    //    //    : TypeDescriptor.GetConverter(t).ConvertFromInvariantString(value);
                    //}
                    else
                    {
                        //Console.WriteLine("2");
                        //safeValue = (value == null) ? null : Convert.ChangeType(value, t);

                        safeValue = (value == null)
                            ? null
                            : TypeDescriptor.GetConverter(t).ConvertFromInvariantString(value);
                    }

                    field.SetValue(this, safeValue, null);
                    //Console.WriteLine("3");
                }
                else
                {
                    Logger?.LogWarning("ModifyField Entity: " + 
                                       GetType().FullName + "." + 
                                       MethodBase.GetCurrentMethod()?.Name +
                                       " - " + fieldName + " - " + value + ": Can not write OR null!");
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex.Source + " " + ex.Message + " " + ex.StackTrace);
            }
        }

        private void AddEntityModifyFieldEvent(string fieldName, string fieldValue)
        {
            var entityModifyField = new EntityModifyField(fieldName, fieldValue, DateTime.Now);

            AddDomainEvent(entityModifyField);
        }
    }
}
