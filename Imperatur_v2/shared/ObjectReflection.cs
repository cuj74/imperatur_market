using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Imperatur_v2.shared
{
    public class ObjectReflection
    {
        private BindingFlags _bindingFlags;
        public ObjectReflection()
        {
            _bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
        }

        public ObjectReflection(BindingFlags bindingFlags)
        {
            _bindingFlags = bindingFlags;
        }

        public List<MemberInfo> GetMemberInfo(object SourceObject, BindingFlags bindingFlags)
        {
            List<MemberInfo> oMembers = SourceObject.GetType().GetFields(bindingFlags).Cast<MemberInfo>()
                .Concat(SourceObject.GetType().GetProperties(bindingFlags)).ToList();
            return oMembers;
        }
        public List<MemberInfo> GetMemberInfo(object SourceObject)
        {
            return GetMemberInfo(SourceObject, _bindingFlags);
        }
        public BindingFlags BindingFlags
        {
            get
            {
                return _bindingFlags;
            }
        }

    }
}
