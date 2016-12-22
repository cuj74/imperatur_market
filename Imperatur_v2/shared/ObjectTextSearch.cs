using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;
using System.Reflection;

namespace Imperatur_v2.shared
{
    public class ObjectTextSearch
    {
        private CultureInfo currentCulture;
        private string[] ObjecttypesToIterate;
        private ObjectReflection m_oObjectReflection;

        public ObjectTextSearch()
        {
            currentCulture = Thread.CurrentThread.CurrentCulture;
            ObjecttypesToIterate = new string[]
            {
                "Customer"
            };
            m_oObjectReflection = new ObjectReflection();
        }

        internal bool FindTextInString(string ToFind, string ToSearch)
        {
            if (ToFind == null || ToSearch == null || ToFind.Trim() == "" || ToSearch.Trim() == "")
                return false;
            return currentCulture.CompareInfo.IndexOf(ToSearch, ToFind, CompareOptions.IgnoreCase) >= 0 ? true : false;
        }

        internal bool FindTextInObject(string ToFind, object ObjectToSearch)
        {
            bool bHit = false;
            foreach (MemberInfo oM in m_oObjectReflection.GetMemberInfo(ObjectToSearch))
            {
                switch (oM.MemberType)
                {
                    case MemberTypes.Constructor:
                        break;
                    case MemberTypes.Event:
                        break;
                    case MemberTypes.Field:
                        FieldInfo oFieldSearchInfo = ObjectToSearch.GetType().GetFields(m_oObjectReflection.BindingFlags).Where(f => f.Name.Equals(oM.Name)).FirstOrDefault();
                        if (oFieldSearchInfo.FieldType.Equals(typeof(String)))
                        {
                            if (oFieldSearchInfo.GetValue(ObjectToSearch) != null)
                            {
                                bHit = FindTextInString(ToFind, oFieldSearchInfo.GetValue(ObjectToSearch).ToString());
                            }
                        }
                        else if (Array.FindIndex(
                            ObjecttypesToIterate,
                            element => element.Equals(oFieldSearchInfo.FieldType.Name)
                            ) >= 0
                            &&
                            oFieldSearchInfo.GetValue(ObjectToSearch) != null
                            )
                        {
                            //iterate this object as well
                            bHit = FindTextInObject(ToFind, oFieldSearchInfo.GetValue(ObjectToSearch));
                        }
                        break;
                    case MemberTypes.Method:
                        break;
                    case MemberTypes.Property:
                        
                        if (ObjectToSearch.GetType().GetProperties(m_oObjectReflection.BindingFlags).Where(p => p.Name.Equals(oM.Name)).FirstOrDefault().PropertyType.Equals(typeof(String)))
                        {
                            if (ObjectToSearch.GetType().GetProperties(m_oObjectReflection.BindingFlags).Where(p => p.Name.Equals(oM.Name)).FirstOrDefault().GetValue(ObjectToSearch) != null)
                            {
                                bHit = FindTextInString(ToFind, ObjectToSearch.GetType().GetProperties(m_oObjectReflection.BindingFlags).Where(p => p.Name.Equals(oM.Name)).FirstOrDefault().GetValue(ObjectToSearch).ToString());
                            }
                        }
                        break;
                    case MemberTypes.TypeInfo:
                        break;
                    case MemberTypes.Custom:
                        break;
                    case MemberTypes.NestedType:
                        break;
                    case MemberTypes.All:
                        break;
                    default:
                        break;
                }
                if (bHit)
                    break;
            }
            return bHit;
        }
    }
}
