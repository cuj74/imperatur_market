using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Imperatur_v2.shared;
using System.Reflection;
using System.Data;

namespace Imperatur_Market_Client.control
{
    public class DataGridForControl
    {
        public string GroupBoxCaption;
        public DataGridView DataGridViewToBuild;
    }

    public class CreateDataGridControlFromObject : UserControl
    {
        public CreateDataGridControlFromObject(DataGridForControl NewDataGridData)
        {
            CreateInfoControlFromObject oC = new CreateInfoControlFromObject();
            GroupBox oB = oC.CreateGroupBox(NewDataGridData.GroupBoxCaption);
            oB.Dock = DockStyle.Fill;
            oB.Controls.Add(NewDataGridData.DataGridViewToBuild);
            this.Dock = DockStyle.Fill;
        }
    }

    public class CreateInfoControlFromObject : UserControl
    {

        private string[] ObjectNamesToShow;

        public CreateInfoControlFromObject()
        {

        }
        public CreateInfoControlFromObject(object ObjectTemplate, string GroupBoxCaption, string[] ObjectNamesToShow)
        {
            this.ObjectNamesToShow = ObjectNamesToShow;
            this.Dock = DockStyle.Fill;
            GroupBox oB = CreateGroupBox(GroupBoxCaption);
            this.Controls.Add(oB);
            Dictionary<string, string> ObjectData = GetDataFromMembers(ObjectTemplate);//, ObjectNamesToShow);

            TableLayoutPanel tlp = CreateTableLayoutPanel(ObjectData.Count);
            int i = 0;
            foreach (KeyValuePair<string, string> pair in ObjectData)
            {
                tlp.Controls.Add(new TextBox()
                {
                    Text = pair.Value,
                    Name = pair.Key,
                    Anchor = AnchorStyles.Left,
                    Width = 200,
                    //AutoSize = true,
                    ReadOnly = true
                }, 1, i);
                tlp.Controls.Add(new Label() { Text = pair.Key, Anchor = AnchorStyles.Left, AutoSize = true }, 0, i);
                i++;
            }
           
            oB.Controls.Add(tlp);
        }

        public GroupBox CreateGroupBox(string Caption)
        {
            return new GroupBox()
            {
                Dock = DockStyle.Fill,
                Text = Caption,
                Name = "gpb",
                Visible = true,
                Padding = new Padding()
                {
                    All = 20
                },
                Margin = new Padding()
                {
                    All = 20
                },
            };
        }

        private TableLayoutPanel CreateTableLayoutPanel(int RowCount = 10)
        {
            return new TableLayoutPanel
            {
                Name = "tlp",
                Dock = DockStyle.Fill,
                RowCount = RowCount,
                ColumnCount = 2,
                Visible = true,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.Single
            };
        }

        private Dictionary<string, string> GetDataFromMembers(Object ReflectedObject, string[] ObjectNamesToInclude)
        {
            Dictionary<string, string> ObjectValues = new Dictionary<string, string>();
            string Value = "";
            string Name = "";
            ObjectReflection oOR = new ObjectReflection();
            foreach (MemberInfo oM in oOR.GetMemberInfo(ReflectedObject))
            {

                if (Array.FindIndex(
                            ObjectNamesToInclude,
                            element => element.Equals(oM.Name)
                            ) >= 0)
                {
                    Value = "";
                    Name = "";
                    switch (oM.MemberType)
                    {
                        case MemberTypes.Constructor:
                            break;
                        case MemberTypes.Event:
                            break;
                        case MemberTypes.Field:
                            FieldInfo oFieldInfo = ReflectedObject.GetType().GetFields(oOR.BindingFlags).Where(f => f.Name.Equals(oM.Name)).FirstOrDefault();

                            if (oFieldInfo.GetValue(ReflectedObject) != null)
                            {
                                Value = oFieldInfo.GetValue(ReflectedObject).ToString();
                                Name = oFieldInfo.Name;
                            }
                            break;
                        case MemberTypes.Method:
                            break;
                        case MemberTypes.Property:
                            PropertyInfo oPropertyInfo = ReflectedObject.GetType().GetProperties(oOR.BindingFlags).Where(f => f.Name.Equals(oM.Name)).FirstOrDefault();
                            if (oPropertyInfo.GetValue(ReflectedObject) != null)
                            {
                                Value = oPropertyInfo.GetValue(ReflectedObject).ToString();
                                Name = oPropertyInfo.Name;
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
                    if (Name != "")
                    {
                        ObjectValues.Add(Name, Value);
                    }
                }

            }
            return ObjectValues;
        }
        private Dictionary<string, string> GetDataFromMembers(Object ReflectedObject)
        {
            Dictionary<string, string> ObjectValues = new Dictionary<string, string>();
            var DReflectedAttribute = ReflectedObject.GetType().GetCustomAttributes(typeof(DesignAttribute),false).FirstOrDefault() as DesignAttribute;
            if (DReflectedAttribute == null) 
            {
                return ObjectValues;
            }
            else if(!DReflectedAttribute.VisibleAtPresentation)
            {
                return ObjectValues;
            }
            string Value = "";
            string Name = "";
            ObjectReflection oOR = new ObjectReflection();
            foreach (MemberInfo oM in oOR.GetMemberInfo(ReflectedObject))
            {
                DesignAttribute oDa = (DesignAttribute)Attribute.GetCustomAttribute(oM, typeof(DesignAttribute));

                if (oDa != null && oDa.VisibleAtPresentation)
                {
                    Value = "";
                    Name = "";
                    switch (oM.MemberType)
                    {
                        case MemberTypes.Constructor:
                            break;
                        case MemberTypes.Event:
                            break;
                        case MemberTypes.Field:
                            FieldInfo oFieldInfo = ReflectedObject.GetType().GetFields(oOR.BindingFlags).Where(f => f.Name.Equals(oM.Name)).FirstOrDefault();
                            if (oDa.Expand && oFieldInfo.GetValue(ReflectedObject) != null)
                            {
                                //get the underlying object to show;
                                foreach (KeyValuePair<string, string> oRec in GetDataFromMembers(oFieldInfo.GetValue(ReflectedObject)))
                                {
                                    ObjectValues.Add(
                                        oRec.Key,
                                        oRec.Value
                                        );
                                    // do something with entry.Value or entry.Key
                                }

                            }
                            else
                            {
                                if (oFieldInfo.GetValue(ReflectedObject) != null)
                                {
                                    Value = oFieldInfo.GetValue(ReflectedObject).ToString();
                                    Name = oFieldInfo.Name;
                                }
                            }
                            break;
                        case MemberTypes.Method:
                            break;
                        case MemberTypes.Property:
                            PropertyInfo oPropertyInfo = ReflectedObject.GetType().GetProperties(oOR.BindingFlags).Where(f => f.Name.Equals(oM.Name)).FirstOrDefault();
                            if (oPropertyInfo.GetValue(ReflectedObject) != null)
                            {
                                Value = oPropertyInfo.GetValue(ReflectedObject).ToString();
                                Name = oPropertyInfo.Name;
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
                    if (Name != "")
                    {
                        ObjectValues.Add(Name, Value);
                    }
                }

            }
            return ObjectValues;
        }
    }
}
