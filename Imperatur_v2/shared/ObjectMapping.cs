using Imperatur_v2.monetary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.shared
{
    public class ObjectMapping
    {
        /// <summary>
        /// List of forced mappings, first string is oR.Name, second is oA.name
        /// Example: BrickId, BFSid
        /// </summary>
        private List<Tuple<string, string>> m_oForcedMappings;
        public ObjectMapping(List<Tuple<string, string>> ForcedMappings)
        {
            m_oForcedMappings = ForcedMappings;
        }

        public ObjectMapping()
        {
        }


        public List<object> GetMappingToObjects(Type TypeOfObject, object[] oR)
        {
            return (
                from r in oR
                select GetMappingToObject(Activator.CreateInstance(TypeOfObject), r)
                ).ToList();
        }

        public object GetMappingToObject(object oA, object oR)
        {
            //each property that should correspond to field
            foreach (PropertyInfo oI in oR.GetType().GetProperties())
            {
                if (m_oForcedMappings != null && m_oForcedMappings.Exists(t => t.Item1.Equals(oI.Name)) && oA.GetType().GetField(oI.Name) == null)
                {
                    oA.GetType().GetField(m_oForcedMappings.Find(t => t.Item1.Equals(oI.Name)).Item2).SetValue(oA, oI.GetValue(oR));
                    continue;
                }

                if (oA.GetType().GetField(oI.Name) != null && oI.GetValue(oR) != null)
                {
                    if (oA.GetType().GetField(oI.Name).FieldType.Equals(oI.GetValue(oR).GetType()))
                        oA.GetType().GetField(oI.Name).SetValue(oA, oI.GetValue(oR));
                    else
                    {
                        if (oA.GetType().GetField(oI.Name).FieldType.IsEnum)
                        {
                            oA.GetType().GetField(oI.Name).SetValue(
                                oA,
                                Enum.Parse(oA.GetType().GetField(oI.Name).FieldType, oI.GetValue(oR).ToString(), true)
                                );
                        }
                        else
                        {
                            switch (oA.GetType().GetField(oI.Name).FieldType.Name)
                            {
                                case "Currency":
                                    oA.GetType().GetField(oI.Name).SetValue(oA, new Currency(oI.GetValue(oR).ToString()));
                                    break;
                                default:
                                    Type t = Nullable.GetUnderlyingType(oA.GetType().GetField(oI.Name).FieldType) ?? oA.GetType().GetField(oI.Name).FieldType;

                                    object safeValue = (oI.GetValue(oR) == null) ? null : Convert.ChangeType(oI.GetValue(oR), t);

                                    oA.GetType().GetField(oI.Name).SetValue(oA, safeValue);
                                    break;

                            }

                        }
                    }
                }
            }
            //each field that corresponds to a field
            foreach (FieldInfo oF in oR.GetType().GetFields())
            {
                if (m_oForcedMappings != null && m_oForcedMappings.Exists(t => t.Item1.Equals(oF.Name)) && oA.GetType().GetField(oF.Name) == null)
                {
                    oA.GetType().GetField(m_oForcedMappings.Find(t => t.Item1.Equals(oF.Name)).Item2).SetValue(oA, oF.GetValue(oR));
                    continue;
                }

                if (oA.GetType().GetField(oF.Name) != null && oF.GetValue(oR) != null)
                {
                    if (oA.GetType().GetField(oF.Name).FieldType.Equals(oF.GetValue(oR).GetType()))
                        oA.GetType().GetField(oF.Name).SetValue(oA, oF.GetValue(oR));
                    else
                    {
                        if (oA.GetType().GetField(oF.Name).FieldType.IsEnum)
                        {
                            oA.GetType().GetField(oF.Name).SetValue(
                                oA,
                                Enum.Parse(oA.GetType().GetField(oF.Name).FieldType, oF.GetValue(oR).ToString(), true)
                                );
                        }
                        else
                        {
                            switch (oA.GetType().GetField(oF.Name).FieldType.Name)
                            {
                                case "Currency":
                                case "CurrencyCode":
                                    oA.GetType().GetField(oF.Name).SetValue(oA, new Currency(oF.GetValue(oR).ToString()));
                                    break;

                                default:
                                    Type t = Nullable.GetUnderlyingType(oA.GetType().GetField(oF.Name).GetType()) ?? oA.GetType().GetField(oF.Name).GetType();

                                    object safeValue = (oF.GetValue(oR) == null) ? null : Convert.ChangeType(oF.GetValue(oR), t);

                                    oA.GetType().GetField(oF.Name).SetValue(oA, safeValue);
                                    break;

                            }

                        }
                    }
                }
            }

            //each field in oR that corresponds to a property in oA

            foreach (FieldInfo oF in oR.GetType().GetFields())
            {
                if (m_oForcedMappings != null && m_oForcedMappings.Exists(t => t.Item1.Equals(oF.Name)) && oA.GetType().GetProperty(oF.Name) == null)
                {
                    oA.GetType().GetProperty(m_oForcedMappings.Find(t => t.Item1.Equals(oF.Name)).Item2).SetValue(oA, oF.GetValue(oR));
                    continue;
                }
                if (oA.GetType().GetProperty(oF.Name) != null && oF.GetValue(oR) != null)
                {
                    if (oA.GetType().GetProperty(oF.Name).PropertyType.Equals(oF.GetValue(oR).GetType()))
                        oA.GetType().GetProperty(oF.Name).SetValue(oA, oF.GetValue(oR));
                    else
                    {
                        if (oA.GetType().GetProperty(oF.Name).PropertyType.IsEnum)
                        {
                            oA.GetType().GetProperty(oF.Name).SetValue(
                                oA,
                                Enum.Parse(oA.GetType().GetProperty(oF.Name).PropertyType, oF.GetValue(oR).ToString(), true)
                                );
                        }
                        else
                        {
                            switch (oA.GetType().GetProperty(oF.Name).Name)
                            {
                                case "CurrencyCode":
                                case "Currency":
                                    if (oA.GetType().GetProperty(oF.Name).PropertyType == typeof(Currency))
                                    {
                                        oA.GetType().GetProperty(oF.Name).SetValue(oA, new Currency(oF.GetValue(oR).ToString()));
                                    }
                                    else
                                        oA.GetType().GetProperty(oF.Name).SetValue(oA, ((Currency)(oF.GetValue(oR))).CurrencyCode);
                                    break;
                                default:
                                    Type t = Nullable.GetUnderlyingType(oA.GetType().GetProperty(oF.Name).PropertyType) ?? oA.GetType().GetProperty(oF.Name).PropertyType;

                                    object safeValue = (oF.GetValue(oR) == null) ? null : Convert.ChangeType(oF.GetValue(oR), t);

                                    oA.GetType().GetProperty(oF.Name).SetValue(oA, safeValue, null);

                                    break;

                            }

                        }
                    }
                }

            }
            return oA;
        }
    }
}
