using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Imperatur_v2.shared;
using System.Reflection;

namespace Imperatur_Market_Client.control
{
    public class CreateControlFromObject : UserControl
    {

        private string[] ObjectNamesToShow;
        public CreateControlFromObject(object ObjectTemplate, string GroupBoxCaption, string[] ObjectNamesToShow)
        {
            this.ObjectNamesToShow = ObjectNamesToShow;
            this.Dock = DockStyle.Fill;
            GroupBox oB = CreateGroupBox(GroupBoxCaption);
            this.Controls.Add(oB);
            Dictionary<string, string> ObjectData = GetDataFromMembers(ObjectTemplate, ObjectNamesToShow);

            TableLayoutPanel tlp = CreateTableLayoutPanel(ObjectData.Count);
            int i = 0;
            foreach (KeyValuePair<string, string> pair in ObjectData)
            {
                tlp.Controls.Add(new TextBox()
                {
                    Text = pair.Value,
                    Name = pair.Key,
                    Anchor = AnchorStyles.Left,
                    Width = 300,
                    AutoSize = true,
                    ReadOnly = true
                }, 0, i);
                tlp.Controls.Add(new Label() { Text = pair.Key, Anchor = AnchorStyles.Left, AutoSize = true }, 1, i);
                i++;
            }
              /*  
            for (int i = 0; i < ObjectData.Count-1; i++)
            {
                tlp.Controls.Add(new TextBox()
                {
                    Text = ObjectData[i].,
                    Name = Name,
                    Anchor = AnchorStyles.Left,
                    Width = 300,
                    AutoSize = true//,
                                   //ReadOnly = prop.Name.Equals("SystemDirectory") && SystemLocation.Length > 0 ? true : false
                }, 0, indexcount);
                tlp.Controls.Add(new Label() { Text = Name, Anchor = AnchorStyles.Left, AutoSize = true }, 1, indexcount);
                indexcount++;
            }
            */

            /*
            string Value = "";
            string Name = "";
            int indexcount = 0;
            ObjectReflection oOR = new ObjectReflection();
            foreach (MemberInfo oM in oOR.GetMemberInfo(ObjectTemplate))
            {
                if (Array.FindIndex(
                            ObjectNamesToShow,
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
                            FieldInfo oFieldInfo = ObjectTemplate.GetType().GetFields(oOR.BindingFlags).Where(f => f.Name.Equals(oM.Name)).FirstOrDefault();

                            if (oFieldInfo.GetValue(ObjectTemplate) != null)
                            {
                                Value = oFieldInfo.GetValue(ObjectTemplate).ToString();
                                Name = oFieldInfo.Name;
                            }
                            break;
                        case MemberTypes.Method:
                            break;
                        case MemberTypes.Property:
                            PropertyInfo oPropertyInfo = ObjectTemplate.GetType().GetProperties(oOR.BindingFlags).Where(f => f.Name.Equals(oM.Name)).FirstOrDefault();
                            if (oPropertyInfo.GetValue(ObjectTemplate) != null)
                            {
                                Value = oPropertyInfo.GetValue(ObjectTemplate).ToString();
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
                        tlp.Controls.Add(new TextBox()
                        {
                            Text = Value,
                            Name = Name,
                            Anchor = AnchorStyles.Left,
                            Width = 300,
                            AutoSize = true//,
                                           //ReadOnly = prop.Name.Equals("SystemDirectory") && SystemLocation.Length > 0 ? true : false
                        }, 0, indexcount);
                        tlp.Controls.Add(new Label() { Text = Name, Anchor = AnchorStyles.Left, AutoSize = true }, 1, indexcount);
                        indexcount++;
                    }
                }

            }*/
            

            oB.Controls.Add(tlp);
        }

        private GroupBox CreateGroupBox(string Caption)
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
    }
}
