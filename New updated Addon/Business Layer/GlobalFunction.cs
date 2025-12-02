using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using SAPbouiCOM;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GardenAPP
{
	//Imports System.IO.StreamReade
	/// <summary>
	/// Globally whatever Function and method do you want define here 
	/// We can use any class and module from here  
	/// </summary>
	/// <remarks></remarks>
	public class GlobalFunctions

	{
        string format = "yyyyMMdd";
        public static string contractNo = "";
        public static int FormNum = 1;
        public Thread ShowFolderBrowserThread { get; private set; }
        #region " ...  Common For Company ..."

        #region Load XML File

        #endregion

   

        public void AddXML(string pathstr)
        {
            try
            {
                System.Xml.XmlDocument xmldoc = new System.Xml.XmlDocument();
                string[] abc = Assembly.GetExecutingAssembly().GetManifestResourceNames();

                System.IO.Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("GardenAPP." + pathstr);
                System.IO.StreamReader streamreader = new System.IO.StreamReader(stream, true);
                xmldoc.LoadXml(streamreader.ReadToEnd());
                streamreader.Close();
                EventHandler.oApplication.LoadBatchActions(xmldoc.InnerXml);
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("AddXML Method Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            finally
            {
            }
        }



        public bool FormExist(string FormID)
		{
			bool functionReturnValue = false;
			functionReturnValue = false;
			foreach (SAPbouiCOM.Form uid in EventHandler.oApplication.Forms) {
				if (uid.UniqueID == FormID) {
					functionReturnValue = true;
					return functionReturnValue;
				}
			}
			if (functionReturnValue) {
				EventHandler.oApplication.Forms.Item(FormID).Visible = true;
				EventHandler.oApplication.Forms.Item(FormID).Select();
			}
			return functionReturnValue;
		}
		public int ConnectionContext()
		{
			int functionReturnValue = 0;
			try {
				int strErrorCode ;
				if (GlobalVariables.oCompany.Connected == true)
					GlobalVariables.oCompany.Disconnect();

				EventHandler.oApplication.StatusBar.SetText("Connecting " + GlobalVariables.addonName + " Addon With The Company..........      Please Wait ..........", SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
				strErrorCode = GlobalVariables.oCompany.Connect();
				functionReturnValue = strErrorCode;

				dynamic a = GlobalVariables.oCompany.GetLastErrorDescription();
				dynamic b = GlobalVariables.oCompany.GetLastErrorCode();


				if (strErrorCode == 0) {
					EventHandler.oApplication.StatusBar.SetText("ADDON for  " + GlobalVariables.addonName + " Module - Connection Established  !!! ", SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                   // System.Media.SystemSounds.Asterisk.Play();
				} else {
					EventHandler.oApplication.StatusBar.SetText("Failed To Connect, Please Check The License Configuration....." + GlobalVariables.oCompany.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Long, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
				}
			} catch (Exception ex) {
				EventHandler.oApplication.StatusBar.SetText(ex.Message);
			} finally {
			}
			return functionReturnValue;
		}
		public int CookieConnect()
		{
			int functionReturnValue = 0;
			try {
				string strCkie = null;
				string strContext = null;
				GlobalVariables.oCompany = new SAPbobsCOM.Company();
				//Debug.Print(GlobalVariables.oCompany.CompanyDB);
				strCkie = GlobalVariables.oCompany.GetContextCookie();
				strContext = EventHandler.oApplication.Company.GetConnectionContext(strCkie);
				functionReturnValue = GlobalVariables.oCompany.SetSboLoginContext(strContext);
			} catch (Exception ex) {
				EventHandler.oApplication.StatusBar.SetText(ex.Message);
			} finally {
			}
			return functionReturnValue;
		}
		public void SetApplication()
		{
			try {
                SAPbouiCOM.SboGuiApi oGUI = null;
                oGUI= new SAPbouiCOM.SboGuiApi();
				oGUI.AddonIdentifier = "";
                //Connection String Coming from project debug properties
                string ConnectionString = Environment.GetCommandLineArgs().GetValue(1).ToString();
                oGUI.Connect(ConnectionString);
				EventHandler.oApplication = oGUI.GetApplication();
			} catch (Exception ex) {
				
                EventHandler.oApplication.StatusBar.SetText(ex.Message);
			} finally {
			}
		}
		#endregion

		#region " ... Menu Creation ..."

		public void LoadXML(SAPbouiCOM.Form Form, string FormId, string FormXML)
		{
			try {
                AddXML(FormXML);
                Form = EventHandler.oApplication.Forms.Item(FormId);

            }
            catch (Exception ex) {
				EventHandler.oApplication.StatusBar.SetText("LoadXML Method Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
			} finally {
			}
		}
       
        #endregion

        #region " ... Common For Data Base Creation ...   "
        public bool UDOExists(string code)
        {
            GC.Collect();
            SAPbobsCOM.UserObjectsMD v_UDOMD = null;
            bool v_ReturnCode = false;
            v_UDOMD = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);
            v_ReturnCode = v_UDOMD.GetByKey(code);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UDOMD);
            v_UDOMD = null;
            return v_ReturnCode;
        }

        public bool CreateTable(string TableName, string TableDesc, SAPbobsCOM.BoUTBTableType TableType)
        {
            bool functionReturnValue = false;
            functionReturnValue = false;
            long v_RetVal = 0;
            int v_ErrCode = 0;
            string v_ErrMsg = "";
            try
            {
                if (!this.TableExists(TableName))
                {
                    SAPbobsCOM.UserTablesMD v_UserTableMD = null;
                    EventHandler.oApplication.StatusBar.SetText("Creating Table " + TableName + " ...................", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                    v_UserTableMD = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
                    v_UserTableMD.TableName = TableName;
                    v_UserTableMD.TableDescription = TableDesc;
                    v_UserTableMD.TableType = TableType;
                    v_RetVal = v_UserTableMD.Add();
                    if (v_RetVal != 0)
                    {
                        GlobalVariables.oCompany.GetLastError(out v_ErrCode, out v_ErrMsg);
                        EventHandler.oApplication.StatusBar.SetText("Failed to Create Table " + TableDesc + v_ErrCode + " " + v_ErrMsg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UserTableMD);
                        v_UserTableMD = null;
                        return false;
                    }
                    else
                    {
                        EventHandler.oApplication.StatusBar.SetText("[" + TableName + "] - " + TableDesc + " Created Successfully!!!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UserTableMD);
                        v_UserTableMD = null;
                        return true;
                    }
                }
                else
                {
                    GC.Collect();
                    return false;
                }
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText(GlobalVariables.addonName + ":> " + ex.Message + " @ " + ex.Source);
            }
            return functionReturnValue;
        }

        public bool ColumnExists(string TableName, string FieldID)
        {
            try
            {
                SAPbobsCOM.Recordset rs = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                bool oFlag = true;
                rs.DoQuery("Select 1 from CUFD Where \"TableID\"='" + Strings.Trim(TableName) + "' and \"AliasID\"='" + Strings.Trim(FieldID) + "'");
                if (rs.EoF)
                    oFlag = false;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
                rs = null;
                GC.Collect();
                return oFlag;
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText(ex.Message);
                return false;
            }
        }

        public bool UDFExists(string TableName, string FieldID)
        {
            try
            {
                SAPbobsCOM.Recordset rs = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                bool oFlag = true;
                rs.DoQuery("Select 1 from CUFD Where \"TableID\"='" + Strings.Trim(TableName) + "' and \"AliasID\"='" + Strings.Trim(FieldID) + "'");
                if (rs.EoF)
                    oFlag = false;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(rs);
                rs = null;
                GC.Collect();
                return oFlag;
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText(ex.Message);
                return false;
            }
        }

        public bool TableExists(string TableName)
        {
            SAPbobsCOM.UserTablesMD oTables = null;
            bool oFlag = false;
            oTables = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);
            oFlag = oTables.GetByKey(TableName);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(oTables);
            return oFlag;
        }
         public void AddControls(SAPbouiCOM.Form objForm,
                 string ItemUID,
                 string SourceUID,
                 SAPbouiCOM.BoFormItemTypes ItemType,
                 string position,
                 int fromPane = 1,
                 int toPane = 1,
                 string linkedUID = "",
                 string strCaption = "",
                 int aWidth = 0)
 {
     SAPbouiCOM.Item objNewItem, objOldItem;
     SAPbouiCOM.StaticText oStatic;
     SAPbouiCOM.Button oButton;
     SAPbouiCOM.CheckBox oCheckbox;

     objOldItem = objForm.Items.Item(SourceUID);
     objNewItem = objForm.Items.Add(ItemUID, ItemType);

     if (ItemType == SAPbouiCOM.BoFormItemTypes.it_LINKED_BUTTON)
     {
         objNewItem.Left = objOldItem.Left - 15;
         objNewItem.Top = objOldItem.Top + 1;
         objNewItem.LinkTo = linkedUID;
     }
     else
     {
         if (position.ToUpper() == "RIGHT")
         {
             objNewItem.Left = objOldItem.Left + objOldItem.Width + 5;
             objNewItem.Top = objOldItem.Top;
             objNewItem.Height = objOldItem.Height;
         }
         else if (position.ToUpper() == "DOWN")
         {
             objNewItem.Top = objOldItem.Top + objOldItem.Height + 1;
             objNewItem.Left = objOldItem.Left;
             objNewItem.Height = objOldItem.Height;
         }
         else if (position.ToUpper() == "TOP")
         {
             objNewItem.Top = objOldItem.Top - objOldItem.Height - 10;
             objNewItem.Left = objOldItem.Left;
             objNewItem.Height = objOldItem.Height;
         }
         else if (position.ToUpper() == "LEFT") // Modified for CheckBox
         {
             objNewItem.Top = objOldItem.Top;
             objNewItem.Left = objOldItem.Left - 30;
             objNewItem.Height = objOldItem.Height;
         }
     }

     objNewItem.FromPane = fromPane;
     objNewItem.ToPane = toPane;

     if (ItemType == SAPbouiCOM.BoFormItemTypes.it_STATIC)
     {
         objNewItem.LinkTo = linkedUID;
     }

     if (ItemType == SAPbouiCOM.BoFormItemTypes.it_EDIT ||
         ItemType == SAPbouiCOM.BoFormItemTypes.it_STATIC ||
         ItemType == SAPbouiCOM.BoFormItemTypes.it_COMBO_BOX)
     {
         objNewItem.Width = objOldItem.Width;
     }

     if (ItemType == SAPbouiCOM.BoFormItemTypes.it_BUTTON)
     {
         objNewItem.Width = objOldItem.Width + 60;
         oButton = (SAPbouiCOM.Button)objNewItem.Specific;
         oButton.Caption = strCaption;
     }
     else if (ItemType == SAPbouiCOM.BoFormItemTypes.it_STATIC)
     {
         oStatic = (SAPbouiCOM.StaticText)objNewItem.Specific;
         oStatic.Caption = strCaption;
     }
     else if (ItemType == SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX)
     {
         oCheckbox = (SAPbouiCOM.CheckBox)objNewItem.Specific;
         oCheckbox.Caption = strCaption;
         objNewItem.Width = objOldItem.Width - 125;
     }

     if (aWidth != 0)
     {
         objNewItem.Width = aWidth;
     }
 }

        public void AddField(string tableName, string columnName, string colDescription, SAPbobsCOM.BoFieldTypes fieldType, int size, SAPbobsCOM.BoFldSubTypes subType, string validValues, string validDescriptions, string setValidValue)
        {
            SAPbobsCOM.UserFieldsMD objUserFieldMD = null;
            try
            {
                string[] strValue = validValues.Split(',');
                string[] strDesc = validDescriptions.Split(',');

                if (strValue.Length != strDesc.Length)
                {
                    throw new Exception("Invalid Valid Values");
                }

                objUserFieldMD = (SAPbobsCOM.UserFieldsMD)GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);

                if (!this.ColumnExists(tableName, columnName))
                {
                    objUserFieldMD.TableName = tableName;
                    objUserFieldMD.Name = columnName;
                    objUserFieldMD.Description = colDescription;
                    objUserFieldMD.Type = fieldType;

                    if (fieldType != SAPbobsCOM.BoFieldTypes.db_Numeric)
                    {
                        objUserFieldMD.Size = size;
                    }
                    else
                    {
                        objUserFieldMD.EditSize = size;
                    }

                    objUserFieldMD.SubType = subType;
                    objUserFieldMD.DefaultValue = setValidValue;

                    for (int i = 0; i < strValue.Length; i++)
                    {
                        objUserFieldMD.ValidValues.Value = strValue[i];
                        objUserFieldMD.ValidValues.Description = strDesc[i];
                        objUserFieldMD.ValidValues.Add();
                    }

                    if (objUserFieldMD.Add() != 0)
                    {
                        // Uncomment for debugging: MessageBox.Show(oApplication.Company.GetLastErrorDescription());
                    }
                }
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("[" + tableName + "] - " + colDescription + " added successfully!!!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (objUserFieldMD != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(objUserFieldMD);
                    objUserFieldMD = null;
                }
                GC.Collect();
            }
        }

        public bool CreateUserFields(string TableName, string FieldName, string FieldDescription, SAPbobsCOM.BoFieldTypes type, int size = 0, SAPbobsCOM.BoFldSubTypes subType = SAPbobsCOM.BoFldSubTypes.st_None, string LinkedTable = "", string DefaultValue = "")
        {
            try
            {
                if (TableName.StartsWith("@") == true)
                {
                    if (!this.ColumnExists(TableName, FieldName))
                    {
                        SAPbobsCOM.UserFieldsMD v_UserField = null;
                        v_UserField = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
                        v_UserField.TableName = TableName;
                        v_UserField.Name = FieldName;
                        v_UserField.Description = FieldDescription;
                        v_UserField.Type = type;
                        if (type != SAPbobsCOM.BoFieldTypes.db_Date)
                        {
                            if (type != SAPbobsCOM.BoFieldTypes.db_Numeric)
                            {
                                if (size != 0)
                                {
                                    v_UserField.Size = size;
                                }
                            }
                            else
                            {
                                v_UserField.EditSize = size;
                            }
                          }
                        if (subType != SAPbobsCOM.BoFldSubTypes.st_None)
                        {
                            v_UserField.SubType = subType;
                        }
                        if (!string.IsNullOrEmpty(LinkedTable))
                            v_UserField.LinkedTable = LinkedTable;
                        if (!string.IsNullOrEmpty(DefaultValue))
                            v_UserField.DefaultValue = DefaultValue;

                        GlobalVariables.v_RetVal = v_UserField.Add();
                        if (GlobalVariables.v_RetVal != 0)
                        {
                            GlobalVariables.oCompany.GetLastError(out GlobalVariables.v_ErrCode, out GlobalVariables.v_ErrMsg);
                            EventHandler.oApplication.StatusBar.SetText("Failed to add UserField masterid" + GlobalVariables.v_ErrCode + " " + GlobalVariables.v_ErrMsg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UserField);
                            v_UserField = null;
                            return false;
                        }
                        else
                        {
                            EventHandler.oApplication.StatusBar.SetText("[" + TableName + "] - " + FieldDescription + " added successfully!!!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UserField);
                            v_UserField = null;
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                if (TableName.StartsWith("@") == false)
                {
                    if (!this.UDFExists(TableName, FieldName))
                    {
                        SAPbobsCOM.UserFieldsMD v_UserField = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);
                        v_UserField.TableName = TableName;
                        v_UserField.Name = FieldName;
                        v_UserField.Description = FieldDescription;
                        v_UserField.Type = type;
                        if (type != SAPbobsCOM.BoFieldTypes.db_Date)
                        {
                            if (size != 0)
                            {
                                v_UserField.Size = size;
                            }
                        }
                        if (subType != SAPbobsCOM.BoFldSubTypes.st_None)
                        {
                            v_UserField.SubType = subType;
                        }
                        if (!string.IsNullOrEmpty(LinkedTable))
                            v_UserField.LinkedTable = LinkedTable;
                        GlobalVariables.v_RetVal = v_UserField.Add();
                        if (GlobalVariables.v_RetVal != 0)
                        {
                            GlobalVariables.oCompany.GetLastError(out GlobalVariables.v_ErrCode, out GlobalVariables.v_ErrMsg);
                            EventHandler.oApplication.StatusBar.SetText("Failed to add UserField " + FieldDescription + " - " + GlobalVariables.v_ErrCode + " " + GlobalVariables.v_ErrMsg, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UserField);
                            v_UserField = null;
                            return false;
                        }
                        else
                        {
                            EventHandler.oApplication.StatusBar.SetText(" & TableName & - " + FieldDescription + " added successfully!!!", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(v_UserField);
                            v_UserField = null;
                            return true;
                        }

                    }
                    else
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.MessageBox(ex.Message);
                return false;
            }
        }

        public bool SetAttachMentFile(SAPbouiCOM.Form oForm, SAPbouiCOM.DBDataSource oDBDSAttch)
        {
            try
            {
                if (string.IsNullOrEmpty(GlobalVariables.oCompany.AttachMentPath))
                {
                    GlobalVariables.oGFun.StatusBarErrorMsg("Attachment folder not defined or has been removed. [Message 131-102]");
                    return false;
                }

                string strFileName = "";
                Thread t = new Thread((ThreadStart)(() =>
                {
                    OpenFileDialog openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
                    NativeWindow nw = new NativeWindow();
                    try
                    {
                        string ProcessServiceName = "SAP Business One";
                        nw.AssignHandle(System.Diagnostics.Process.GetProcessesByName(ProcessServiceName)[0].MainWindowHandle);
                        EventHandler.oApplication.StatusBar.SetText("If 'Open File Dialog' is not shown, press Alt + Tab", SAPbouiCOM.BoMessageTime.bmt_Short);

                        DialogResult result = openFileDialog1.ShowDialog(nw);
                        if (result == DialogResult.OK)
                        {
                            strFileName = openFileDialog1.FileName;
                        }
                    }
                    catch (Exception ex)
                    {
                        EventHandler.oApplication.StatusBar.SetText("Error in file dialog: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    }
                    finally
                    {
                        nw.ReleaseHandle(); // Properly release the window handle
                    }
                }));
                t.SetApartmentState(ApartmentState.STA);
                t.Start();
                t.Join();
                t = null; // Let the thread go out of scope for GC to collect


                if (string.IsNullOrEmpty(strFileName))
                {
                    return false; // No file selected
                }

                string fileNameOnly = Path.GetFileName(strFileName);
                string fileExt = Path.GetExtension(strFileName);
                string fileDestPath = Path.Combine(GlobalVariables.oCompany.AttachMentPath, fileNameOnly);

                // Check if the file already exists
                if (File.Exists(fileDestPath))
                {
                    long userResponse = EventHandler.oApplication.MessageBox(
                        $"A file with this name already exists. Do you want to replace it?\n\n{fileDestPath} will be replaced.",
                        1, "Yes", "No");
                    if (userResponse != 1) return false;
                }

                try
                {
                    File.Copy(strFileName, fileDestPath, true);
                    oDBDSAttch.SetValue("U_Attachment", 0, fileDestPath);
                    //oDBDSAttch.SetValue("U_TrgtPath", oDBDSAttch.Offset, GlobalVariables.oCompany.AttachMentPath);
                   

                }
                catch (IOException e)
                {
                    EventHandler.oApplication.StatusBar.SetText("File copy failed: " + e.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    return false;
                }

                if (oForm.Mode != SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE;

                return true;
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("SetAttachMentFile Failed: " + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                return false;
            }
        }


        public bool RegisterUDO(string UDOCode, string UDOName, SAPbobsCOM.BoUDOObjType UDOType, string[,] FindField, string UDOHTableName, string UDODTableName = "", string ChildTable = "", string ChildTable1 = "", string ChildTable2 = "", string ChildTable3 = "",
        string ChildTable4 = "", string ChildTable5 = "", string ChildTable6 = "", string ChildTable7 = "", string ChildTable8 = "", string ChildTable9 = "", SAPbobsCOM.BoYesNoEnum LogOption = SAPbobsCOM.BoYesNoEnum.tNO)
        {
            bool functionReturnValue = false;
            bool ActionSuccess = false;
            try
            {
                functionReturnValue = false;
                SAPbobsCOM.UserObjectsMD v_udoMD = null;
                v_udoMD = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);
                v_udoMD.CanCancel = SAPbobsCOM.BoYesNoEnum.tYES;
                v_udoMD.CanClose = SAPbobsCOM.BoYesNoEnum.tYES;
                v_udoMD.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tNO;
                v_udoMD.CanDelete = SAPbobsCOM.BoYesNoEnum.tYES;
                v_udoMD.CanFind = SAPbobsCOM.BoYesNoEnum.tYES;
                v_udoMD.CanLog = SAPbobsCOM.BoYesNoEnum.tYES;
                v_udoMD.CanYearTransfer = SAPbobsCOM.BoYesNoEnum.tYES;
                v_udoMD.ManageSeries = SAPbobsCOM.BoYesNoEnum.tYES;
                v_udoMD.Code = UDOCode;
                v_udoMD.Name = UDOName;
                v_udoMD.TableName = UDOHTableName;
                v_udoMD.CanCreateDefaultForm = SAPbobsCOM.BoYesNoEnum.tYES;
                if (!string.IsNullOrEmpty(UDODTableName))
                {
                    v_udoMD.ChildTables.TableName = UDODTableName;
                    v_udoMD.ChildTables.Add();
                }

                if (!string.IsNullOrEmpty(ChildTable))
                {
                    v_udoMD.ChildTables.TableName = ChildTable;
                    v_udoMD.ChildTables.Add();
                }
                if (!string.IsNullOrEmpty(ChildTable1))
                {
                    v_udoMD.ChildTables.TableName = ChildTable1;
                    v_udoMD.ChildTables.Add();
                }
                if (!string.IsNullOrEmpty(ChildTable2))
                {
                    v_udoMD.ChildTables.TableName = ChildTable2;
                    v_udoMD.ChildTables.Add();
                }
                if (!string.IsNullOrEmpty(ChildTable3))
                {
                    v_udoMD.ChildTables.TableName = ChildTable3;
                    v_udoMD.ChildTables.Add();
                }
                if (!string.IsNullOrEmpty(ChildTable4))
                {
                    v_udoMD.ChildTables.TableName = ChildTable4;
                    v_udoMD.ChildTables.Add();
                }
                if (!string.IsNullOrEmpty(ChildTable5))
                {
                    v_udoMD.ChildTables.TableName = ChildTable5;
                    v_udoMD.ChildTables.Add();
                }
                if (!string.IsNullOrEmpty(ChildTable6))
                {
                    v_udoMD.ChildTables.TableName = ChildTable6;
                    v_udoMD.ChildTables.Add();
                }
                if (!string.IsNullOrEmpty(ChildTable7))
                {
                    v_udoMD.ChildTables.TableName = ChildTable7;
                    v_udoMD.ChildTables.Add();
                }
                if (!string.IsNullOrEmpty(ChildTable8))
                {
                    v_udoMD.ChildTables.TableName = ChildTable8;
                    v_udoMD.ChildTables.Add();
                }
                if (!string.IsNullOrEmpty(ChildTable9))
                {
                    v_udoMD.ChildTables.TableName = ChildTable9;
                    v_udoMD.ChildTables.Add();
                }

                if (LogOption == SAPbobsCOM.BoYesNoEnum.tYES)
                {
                    v_udoMD.CanLog = SAPbobsCOM.BoYesNoEnum.tYES;
                    v_udoMD.LogTableName = "A" + UDOHTableName;
                }
                v_udoMD.ObjectType = UDOType;
                for (Int16 i = 0; i <= FindField.GetLength(0) - 1; i++)
                {
                    if (i > 0)
                        v_udoMD.FindColumns.Add();
                    v_udoMD.FindColumns.ColumnAlias = FindField[i, 0];
                    v_udoMD.FindColumns.ColumnDescription = FindField[i, 1];
                }

                if (v_udoMD.Add() == 0)
                {
                    functionReturnValue = true;
                    if (GlobalVariables.oCompany.InTransaction)
                        GlobalVariables.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                    EventHandler.oApplication.StatusBar.SetText("Successfully Registered UDO >" + UDOCode + ">" + UDOName + " >" + GlobalVariables.oCompany.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }
                else
                {
                    EventHandler.oApplication.StatusBar.SetText("Failed to Register UDO >" + UDOCode + ">" + UDOName + " >" + GlobalVariables.oCompany.GetLastErrorDescription(), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    MessageBox.Show(GlobalVariables.oCompany.GetLastErrorDescription());
                    functionReturnValue = false;
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(v_udoMD);
                v_udoMD = null;
                GC.Collect();
                if (ActionSuccess == false & GlobalVariables.oCompany.InTransaction)
                    GlobalVariables.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
            }
            catch (Exception)
            {
                if (GlobalVariables.oCompany.InTransaction)
                    GlobalVariables.oCompany.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
            }
            return functionReturnValue;
        }
        public void ShowFolderBrowser()
        {
            System.Diagnostics.Process[] MyProcs = null;
            GlobalVariables.BankFileName = "";
            OpenFileDialog OpenFile = new OpenFileDialog();
            try
            {
                OpenFile.Multiselect = false;
                OpenFile.Filter = "All files(*.)|*.*";
                //   "|*.*"

                int filterindex = 0;
                try
                {
                    filterindex = 0;
                }
                catch (Exception ex)
                {
                }
                OpenFile.FilterIndex = filterindex;
                OpenFile.RestoreDirectory = true;
                //Dim sPath As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                //If sPath.Equals("") = False Then OpenFile.InitialDirectory = sPath

                MyProcs = Process.GetProcessesByName("SAP Business One");
                if (MyProcs.Length > 0)
                {
                    for (int i = 0; i <= MyProcs.Length - 1; i++)
                    {
                        WindowWrapper MyWindow = new WindowWrapper(MyProcs[i].MainWindowHandle);
                        DialogResult ret = OpenFile.ShowDialog(MyWindow);
                        if (ret == DialogResult.OK)
                        {
                            GlobalVariables.BankFileName = OpenFile.FileName;
                            GlobalVariables.FileName = OpenFile.SafeFileName;
                            OpenFile.Dispose();
                            break; // TODO: might not be correct. Was : Exit Try
                        }
                        else
                        {
                            System.Windows.Forms.Application.ExitThread();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText(ex.Message);
                GlobalVariables.BankFileName = "";
            }
            finally
            {
                OpenFile.Dispose();
            }
        }

        public void ChooseFromListVendor(SAPbouiCOM.Form oForm, string strCFL_ID)
        {
            try
            {
                SAPbouiCOM.ChooseFromList oCFL = oForm.ChooseFromLists.Item(strCFL_ID);
                SAPbouiCOM.Conditions oConds = null;
                SAPbouiCOM.Condition oCond = null;
                SAPbouiCOM.Conditions oEmptyConds = new SAPbouiCOM.Conditions();
                oCFL.SetConditions(oEmptyConds);
                oConds = oCFL.GetConditions();
                oCond = oConds.Add();
                oCond.Alias = "CardType";
                oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                oCond.CondVal = "S";
                oCFL.SetConditions(oConds);
                oCond.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;
                oCond = oConds.Add();
                oCond.Alias = "frozenFor";
                oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                oCond.CondVal = "N";
                oCFL.SetConditions(oConds);

            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("Choose FromList Filter Global Fun. Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            finally
            {
            }
        }

        public void ChooseFromListDimension(SAPbouiCOM.Form oForm, string strCFL_ID)
        {
            try
            {
                SAPbouiCOM.ChooseFromList oCFL = oForm.ChooseFromLists.Item(strCFL_ID);
                SAPbouiCOM.Conditions oConds = null;
                SAPbouiCOM.Condition oCond = null;
                SAPbouiCOM.Conditions oEmptyConds = new SAPbouiCOM.Conditions();
                oCFL.SetConditions(oEmptyConds);
                oConds = oCFL.GetConditions();
                oCond = oConds.Add();
                oCond.Alias = "DimCode";
                oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                oCond.CondVal = "1";
                oCFL.SetConditions(oConds);
                oCond.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;
                oCond = oConds.Add();
                oCond.Alias = "Active";
                oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                oCond.CondVal = "Y";
                oCFL.SetConditions(oConds);

            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("Choose FromList Filter Global Fun. Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            finally
            {
            }
        }
        public void ChooseFromListCustomer(SAPbouiCOM.Form oForm, string strCFL_ID)
        {
            try
            {
                SAPbouiCOM.ChooseFromList oCFL = oForm.ChooseFromLists.Item(strCFL_ID);
                SAPbouiCOM.Conditions oConds = null;
                SAPbouiCOM.Condition oCond = null;
                SAPbouiCOM.Conditions oEmptyConds = new SAPbouiCOM.Conditions();
                oCFL.SetConditions(oEmptyConds);
                oConds = oCFL.GetConditions();
                oCond = oConds.Add();
                oCond.Alias = "CardType";
                oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                oCond.CondVal = "C";
                oCFL.SetConditions(oConds);

            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("Choose FromList Filter Global Fun. Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            finally
            {
            }
        }
        #endregion

        #region "       Functions  & Methods            "

        public bool GenerateContract(String bankCode, string customerCode, string customerName, double exchangeRate, double amount, DateTime dealDate,string docNum, string objectType)
        {
            bool functionReturnValue = false;
            try
            {
                SAPbobsCOM.GeneralService oGeneralService = null;
                SAPbobsCOM.GeneralData oGeneralData = null;
                SAPbobsCOM.GeneralDataParams oGeneralParams = null;
                SAPbobsCOM.CompanyService sCmp = null;
                SAPbobsCOM.GeneralData oChild = null;
                SAPbobsCOM.GeneralDataCollection oChildren = null;
                sCmp = GlobalVariables.oCompany.GetCompanyService();
                try
                {
                    oGeneralService = sCmp.GetGeneralService("OODCT");
                    // Get UDO record   
                    string query = string.Format("SELECT Top 1 a.[LineId],a.[DocEntry],a.[U_Cntct] FROM [@DCT1] a  Order by a.[LineId] Desc").Replace("[", "\"").Replace("]", "\"");
                    SAPbobsCOM.Recordset queryRec = GlobalVariables.oGFun.DoQuery(query);
                    string queryTwo = string.Format("SELECT Top 1 [U_RefNo],[DocEntry] FROM [@DCT1] WHERE Month(Current_date)=('" + DateTime.Now.Month + "')  Order by [LineId] Desc").Replace("[", "\"").Replace("]", "\"");
                    SAPbobsCOM.Recordset queryTwoRec = GlobalVariables.oGFun.DoQuery(queryTwo);
                    string bankRate = string.Format("SELECT [U_BnkRat] FROM [@BNR1] WHERE [U_Bank]='" + bankCode + "' ").Replace("[", "\"").Replace("]", "\"");
                    SAPbobsCOM.Recordset bankRateRec = GlobalVariables.oGFun.DoQuery(bankRate);
                    string customerPrice = string.Format("SELECT [U_Pric] FROM [@DCS1] a WHERE [U_PrtId] = '" + customerCode + "'").Replace("[", "\"").Replace("]", "\"");
                    SAPbobsCOM.Recordset customerPriceRec = GlobalVariables.oGFun.DoQuery(customerPrice);
                    oGeneralParams = ((SAPbobsCOM.GeneralDataParams)(oGeneralService.GetDataInterface(SAPbobsCOM.GeneralServiceDataInterfaces.gsGeneralDataParams)));
                    oGeneralParams.SetProperty("DocEntry", queryRec.Fields.Item("DocEntry").Value);
                    oGeneralData = oGeneralService.GetByParams(oGeneralParams);
                    // Add lines on UDO Child Table              
                    oChildren = oGeneralData.Child("DCT1");
                    // Update an existing line
                    oChild = oChildren.Add();
                    oChild.SetProperty("U_PrtyId", customerCode);
                    oChild.SetProperty("U_PrtyNm", customerName);
                    oChild.SetProperty("U_ExcRat", exchangeRate);
                    Double contract = Convert.ToDouble(queryRec.Fields.Item("U_Cntct").Value) + 1;
                    oChild.SetProperty("U_Cntct", Convert.ToString((contract)));
                    GlobalVariables.contractNo = Convert.ToString(contract);
                    //double contractAmount = (((amount + (amount * Convert.ToDouble(bankRateRec.Fields.Item("U_BnkRat").Value))) / exchangeRate) / 1000) * 1000;
                    double contractAmount = (amount + (amount * .1));
                    oChild.SetProperty("U_TotCnM", Convert.ToString(contractAmount / Convert.ToDouble(customerPriceRec.Fields.Item("U_Pric").Value)));
                    oChild.SetProperty("U_TotCnAm", contractAmount.ToString());
                   // oChild.SetProperty("U_Packng", "500 Meters = 1 ");
                    oChild.SetProperty("U_Packng", "500 Meters = 1 Roll (Export Packing)");
                    oChild.SetProperty("U_RefNo", Convert.ToString((Convert.ToDouble(queryTwoRec.Fields.Item("U_RefNo").Value) + 1)));
                    oChild.SetProperty("U_ConDat", DateTime.ParseExact(dealDate.AddMonths(-1).ToString("yyyyMMdd"), format, System.Globalization.CultureInfo.InvariantCulture));
                    DateTime negDate = dealDate.AddMonths(2);
                    DateTime newNegDate;
                    if (Convert.ToInt16(negDate.Day) <= 15)
                    {
                        newNegDate = negDate.AddDays(-Convert.ToDouble(negDate.Day)).AddDays(15);
                        oChild.SetProperty("U_NegDat", DateTime.ParseExact(GetNextWorkingDate(newNegDate), format, System.Globalization.CultureInfo.InvariantCulture));
                        oChild.SetProperty("U_ShDt", DateTime.ParseExact(GetNextWorkingDate(newNegDate.AddDays(-14)), format, System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else if (Convert.ToInt16(negDate.Day) > 15)
                    {
                        newNegDate = negDate.AddDays(-Convert.ToDouble(negDate.Day)).AddDays(30);
                        oChild.SetProperty("U_NegDat", DateTime.ParseExact(GetNextWorkingDate(newNegDate), format, System.Globalization.CultureInfo.InvariantCulture));
                        oChild.SetProperty("U_ShDt", DateTime.ParseExact(GetNextWorkingDate(newNegDate.AddDays(-14)), format, System.Globalization.CultureInfo.InvariantCulture));
                    }
                    oChild.SetProperty("U_CovDat", DateTime.ParseExact(DateTime.Now.ToString("yyyyMMdd"), format, System.Globalization.CultureInfo.InvariantCulture));
                    oChild.SetProperty("U_LoanTp", objectType);
                    oChild.SetProperty("U_DocNm", docNum);
                    //Update the UDO Record
                    oGeneralService.Update(oGeneralData);
                    EventHandler.oApplication.StatusBar.SetText("Dummy Contract Generated with Contract No. '" + Convert.ToString((Convert.ToDouble(queryRec.Fields.Item("U_Cntct").Value) + 1))
                        + "' ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                    functionReturnValue = true;
                }
                catch (Exception ex)
                {
                    EventHandler.oApplication.MessageBox(ex.Message, 1, "OK", null, null);
                }

            }
            catch (Exception ex)
            {
                EventHandler.oApplication.MessageBox(ex.Message);
                functionReturnValue = false;
            }
            finally
            {
            }
            return functionReturnValue;
        }

        public string GetServerDate()
		{
			try {
				SAPbobsCOM.SBObob rsetBob = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);
				SAPbobsCOM.Recordset rsetServerDate = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

				rsetServerDate = rsetBob.Format_StringToDate(EventHandler.oApplication.Company.ServerDate);

				return Convert.ToDateTime(rsetServerDate.Fields.Item(0).Value).ToString("yyyyMMdd");

			} catch (Exception ex) {
				GlobalVariables.oGFun.StatusBarErrorMsg("Get Server Date Function Failed : " + ex.Message);
				return "";
			} finally {
			}
		}
        //public string GetServerTime()
        //{
        //    try
        //    {
        //        SAPbobsCOM.SBObob rsetBob = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoBridge);
        //        SAPbobsCOM.Recordset rsetServerDate = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

        //        rsetServerDate = rsetBob.Format_DateToString(EventHandler.oApplication.Company.ServerTime);

        //        return Convert.ToDateTime(rsetServerDate.Fields.Item(0).Value).ToString("hhmm");

        //    }
        //    catch (Exception ex)
        //    {
        //        GlobalVariables.oGFun.StatusBarErrorMsg("Get Server Date Function Failed : " + ex.Message);
        //        return "";
        //    }
        //    finally
        //    {
        //    }
        //}
        public string GetNextWorkingDate(DateTime date)
        {
            try
            {
                DateTime workingDate = date;
                string sDay = date.DayOfWeek.ToString();
                if (sDay == "Sunday")
                {
                    //EventHandler.oApplication.MessageBox("Because of Sunday Date Shifted to Next Working Day ...!");
                    workingDate = date.AddDays(1);
                }
                else if (sDay == "Saturday") {
                    //EventHandler.oApplication.MessageBox("Because of Saturday Date Shifted to Next Working Day ...!");
                    workingDate = date.AddDays(2);
                }
                return Convert.ToDateTime(workingDate).ToString("yyyyMMdd");
            }
            catch (Exception ex)
            {
                GlobalVariables.oGFun.StatusBarErrorMsg("Next Working date : " + ex.Message);
                return "";
            }
            finally
            {
            }
        }
        public string GetPreviousWorkingDate(DateTime date)
        {
            try
            {
                DateTime workingDate = date;
                string sDay = date.DayOfWeek.ToString();
                if (sDay == "Sunday")
                {
                    //EventHandler.oApplication.MessageBox("Because of Sunday Date Shifted to Next Working Day ...!");
                    workingDate = date.AddDays(1);
                }
                else if (sDay == "Saturday")
                {
                    //EventHandler.oApplication.MessageBox("Because of Saturday Date Shifted to Next Working Day ...!");
                    workingDate = date.AddDays(2);
                }
                return Convert.ToDateTime(workingDate).ToString("yyyyMMdd");
            }
            catch (Exception ex)
            {
                GlobalVariables.oGFun.StatusBarErrorMsg("Next Working date : " + ex.Message);
                return "";
            }
            finally
            {
            }
        }
        public string GetDay(DateTime date)
        {
            try
            {
                string sDay = date.DayOfWeek.ToString();
               return sDay;
            }
            catch (Exception ex)
            {
                GlobalVariables.oGFun.StatusBarErrorMsg("Next Working date : " + ex.Message);
                return "";
            }
            finally
            {
            }
        }
        public string GetMatrixValues(SAPbouiCOM.Matrix aMatrix, string colUID, int intRow)
        {
            return aMatrix.Columns.Item(colUID).Cells.Item(intRow).Specific.Value.ToString();
        }

        public void SetMatrixValues(SAPbouiCOM.Matrix aMatrix, string colUID, int intRow, string strValue)
        {
            aMatrix.Columns.Item(colUID).Cells.Item(intRow).Specific.Value = strValue;
        }

        public SAPbobsCOM.Recordset DoQuery(string strSql)
		{
			try {
				SAPbobsCOM.Recordset rsetCode = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
				rsetCode.DoQuery(strSql);
				return rsetCode;
			} catch (Exception ex) {
				EventHandler.oApplication.StatusBar.SetText("Execute Query Function Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
				return null;
			} finally {
			}
		}
        public string GetEditTextValue(SAPbouiCOM.Form aform, string UID)
        {
            SAPbouiCOM.EditText objEdit = (SAPbouiCOM.EditText)aform.Items.Item(UID).Specific;
            return objEdit.String;
        }

        public void SetEditTextValue(SAPbouiCOM.Form aform, string UID, string newValue)
        {
            SAPbouiCOM.EditText objEdit = (SAPbouiCOM.EditText)aform.Items.Item(UID).Specific;
            try
            {
                objEdit.String = newValue;
            }
            catch (Exception ex)
            {
                objEdit.Value = newValue;
            }
        }


        public void LoadComboBoxTarget(SAPbouiCOM.ComboBox oComboBox, string strQry, SAPbobsCOM.Recordset rsetValidValue)
        {
            try
            {
                while (oComboBox.ValidValues.Count > 0)
                {
                    oComboBox.ValidValues.Remove(0, SAPbouiCOM.BoSearchKey.psk_Index);
                }
                if (oComboBox.ValidValues.Count == 0)
                {
                     rsetValidValue = (SAPbobsCOM.Recordset)GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    //Dim strQry As String = "SELECT Code , Location FROM OLCT"

                    rsetValidValue.DoQuery(strQry);
                    rsetValidValue.MoveFirst();
                    for (int j = 0; j <= rsetValidValue.RecordCount - 1; j++)
                    {
                        oComboBox.ValidValues.Add(Convert.ToString(rsetValidValue.Fields.Item(0).Value), Convert.ToString(rsetValidValue.Fields.Item(1).Value));
                        rsetValidValue.MoveNext();
                    }
                    oComboBox.ValidValues.Add("New", "Define New");
                }

            }
            catch (Exception ex)
            {
                GlobalVariables.oGFun.StatusBarErrorMsg("SetComboBoxValueRefresh Method Faild : " + ex.Message);
            }
            finally
            {
            }
        }
        public void LoadComboBox(SAPbouiCOM.ComboBox oComboBox, string strQry)
        {
            try
            {
                while (oComboBox.ValidValues.Count > 0)
                {
                    oComboBox.ValidValues.Remove(0, SAPbouiCOM.BoSearchKey.psk_Index);
                }
                if (oComboBox.ValidValues.Count == 0)
                {
                    SAPbobsCOM.Recordset rsetValidValue = (SAPbobsCOM.Recordset)GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    //Dim strQry As String = "SELECT Code , Location FROM OLCT"

                    rsetValidValue.DoQuery(strQry);
                    rsetValidValue.MoveFirst();
                    oComboBox.ValidValues.Add("", "");
                    for (int j = 0; j <= rsetValidValue.RecordCount - 1; j++)
                    {
                        oComboBox.ValidValues.Add(Convert.ToString(rsetValidValue.Fields.Item(0).Value), Convert.ToString(rsetValidValue.Fields.Item(1).Value));
                        rsetValidValue.MoveNext();
                    }
                }

            }
            catch (Exception ex)
            {
               // GlobalVariables.oGFun.StatusBarErrorMsg("SetComboBoxValueRefresh Method Faild : " + ex.Message);
            }
            finally
            {
            }
        }
        public void LoadCompanyComboBox(SAPbouiCOM.ComboBox oComboBox, SAPbobsCOM.Recordset rsetValidValue)
        {
            try
            {
                    for (int j = 0; j <= rsetValidValue.RecordCount - 1; j++)
                    {
                        oComboBox.ValidValues.Add(Convert.ToString(rsetValidValue.Fields.Item(0).Value), Convert.ToString(rsetValidValue.Fields.Item(1).Value));
                        rsetValidValue.MoveNext();
                    }
                }

            
            catch (Exception ex)
            {
                // GlobalVariables.oGFun.StatusBarErrorMsg("SetComboBoxValueRefresh Method Faild : " + ex.Message);
            }
            finally
            {
            }
        }
        public bool removeValidValues(SAPbouiCOM.ComboBox _combo)
        {
            try
            {
                while (_combo.ValidValues.Count > 0)
                {
                    _combo.ValidValues.Remove(0, SAPbouiCOM.BoSearchKey.psk_Index);
                }
                return true;
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                return false;
            }
        }
        public bool setComboBoxInvoiceStype(SAPbouiCOM.ComboBox oComboBox, string strQry)
        {
            try
            {
                SAPbobsCOM.Recordset rsetValidValue = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                if (oComboBox.ValidValues.Count == 0)
                {
                    rsetValidValue.DoQuery(strQry);
                    rsetValidValue.MoveFirst();
                    for (int j = 0; j <= rsetValidValue.RecordCount - 1; j++)
                    {
                        oComboBox.ValidValues.Add(rsetValidValue.Fields.Item(0).Value, rsetValidValue.Fields.Item(1).Value);
                        rsetValidValue.MoveNext();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("setComboBoxValue Function Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                return true;
            }
            finally
            {
            }

        }
        public bool setComboBoxCurrency(SAPbouiCOM.ComboBox oComboBox, string strQry)
        {
            try
            {
                SAPbobsCOM.Recordset rsetValidValue = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                if (oComboBox.ValidValues.Count == 0)
                {
                    rsetValidValue.DoQuery(strQry);
                    rsetValidValue.MoveFirst();
                    for (int j = 0; j <= rsetValidValue.RecordCount - 1; j++)
                    {
                        oComboBox.ValidValues.Add(rsetValidValue.Fields.Item(0).Value, rsetValidValue.Fields.Item(1).Value);
                        rsetValidValue.MoveNext();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("setComboBoxValue Function Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                return true;
            }
            finally
            {
            }

        }
        public bool setComboBoxBranches(SAPbouiCOM.ComboBox oComboBox, string strQry)
		{
			try {
				SAPbobsCOM.Recordset rsetValidValue = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

				if (oComboBox.ValidValues.Count == 0) {
					rsetValidValue.DoQuery(strQry);
					rsetValidValue.MoveFirst();
					for (int j = 0; j <= rsetValidValue.RecordCount - 1; j++) {
						oComboBox.ValidValues.Add(rsetValidValue.Fields.Item(0).Value, rsetValidValue.Fields.Item(1).Value);
						rsetValidValue.MoveNext();
					}
				}
                return true;
			} catch (Exception ex) {
				EventHandler.oApplication.StatusBar.SetText("setComboBoxValue Function Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
				return true;
			} finally {
			}

		}
        public bool setComboBoxSeriesBranches(SAPbouiCOM.ComboBox oComboBox, string strQry, SAPbouiCOM.DBDataSource oDBDSHeader)
        {
            try
            {
                SAPbobsCOM.Recordset rsetValidValue = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                removeValidValues(oComboBox);
                if (oComboBox.ValidValues.Count == 0)
                {
                    rsetValidValue.DoQuery(strQry);
                    rsetValidValue.MoveFirst();
                    for (int j = 0; j <= rsetValidValue.RecordCount - 1; j++)
                    {
                        oComboBox.ValidValues.Add(rsetValidValue.Fields.Item(0).Value, rsetValidValue.Fields.Item(1).Value);
                        rsetValidValue.MoveNext();
                    }
                    oComboBox.Select(0, SAPbouiCOM.BoSearchKey.psk_Index);
                    string oQ = string.Format("SELECT T0.[NextNumber] FROM NNM1 T0 WHERE T0.[ObjectCode]  = 'ZXOUT' and  T0.[Series] = '" + oDBDSHeader.GetValue("Series", 0) + "'").Replace("[", "\"").Replace("]", "\"");
                    SAPbobsCOM.Recordset oRec = GlobalVariables.oGFun.DoQuery(oQ);
                    oDBDSHeader.SetValue("DocNum", 0, Convert.ToString(oRec.Fields.Item(0).Value));
                }
                return true;
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("setComboBoxValue Function Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                return true;
            }
            finally
            {
            }

        }
        public bool setComboBox(SAPbouiCOM.ComboBox oComboBox, SAPbobsCOM.Recordset rsetValidValue)
        {
            try
            {
    
                    rsetValidValue.MoveFirst();
                    for (int j = 0; j <= rsetValidValue.RecordCount - 1; j++)
                    {
                        oComboBox.ValidValues.Add(rsetValidValue.Fields.Item(0).Value, rsetValidValue.Fields.Item(1).Value);
                        rsetValidValue.MoveNext();
                    }
                
                return true;
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("setComboBoxValue Function Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                return true;
            }
            finally
            {
            }

        }
        public int GetCodeGeneration(string TableName)
		{
			try {
				SAPbobsCOM.Recordset rsetCode = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string oServertype = Convert.ToString(GlobalVariables.oCompany.DbServerType);
                string strCode = "";
                if (oServertype.Contains("SQL"))
                {
                     strCode = "Select ISNULL(Max(ISNULL(\"DocEntry\",0)),0) + 1 Code From \"" + Strings.Trim(TableName) + "\"";
                }
                else {

                     strCode = "Select IFNULL(Max(IFNULL(\"DocEntry\",0)),0) + 1 Code From \"" + Strings.Trim(TableName) + "\"";
                }
               
				rsetCode.DoQuery(strCode);
				return Convert.ToInt32(rsetCode.Fields.Item("Code").Value);
			} catch (Exception ex) {
				EventHandler.oApplication.StatusBar.SetText("GetCodeGeneration Function Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
				return -1;
			} finally {
			}
		}
		public int GetCodeGenerationMaster(string TableName)
		{
			try {
				SAPbobsCOM.Recordset rsetCode = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
				string strCode = "Select IFNULL(Max(IFNULL(\"Code\",0)),0) + 1 Code From \"" + Strings.Trim(TableName) + "\"";
				rsetCode.DoQuery(strCode);
				return Convert.ToInt32(rsetCode.Fields.Item("Code").Value);
			} catch (Exception ex) {
				EventHandler.oApplication.StatusBar.SetText("GetCodeGeneration Function Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
				return -1;
			} finally {
			}
		}
		public void SetNewLine(SAPbouiCOM.Matrix oMatrix, SAPbouiCOM.DBDataSource oDBDSDetail, int RowID = 1, string ColumnUID = "")
		{
			try {
				if (ColumnUID.Equals("") == false) {
					if (oMatrix.VisualRowCount > 0) {
						if (oMatrix.Columns.Item(ColumnUID).Cells.Item(RowID).Specific.Value.Equals("") == false & RowID == oMatrix.VisualRowCount) {
							oMatrix.FlushToDataSource();
							oMatrix.AddRow();
							oDBDSDetail.InsertRecord(oDBDSDetail.Size);
							oDBDSDetail.Offset = oMatrix.VisualRowCount - 1;
							oDBDSDetail.SetValue("LineID", oDBDSDetail.Offset, oMatrix.VisualRowCount.ToString());

							oMatrix.SetLineData(oMatrix.VisualRowCount);
							oMatrix.FlushToDataSource();
						}
					} else {
						oMatrix.FlushToDataSource();
						oMatrix.AddRow();
						oDBDSDetail.InsertRecord(oDBDSDetail.Size);
						oDBDSDetail.Offset = oMatrix.VisualRowCount - 1;
						oDBDSDetail.SetValue("LineID", oDBDSDetail.Offset, oMatrix.VisualRowCount.ToString());

						oMatrix.SetLineData(oMatrix.VisualRowCount);
						oMatrix.FlushToDataSource();
					}

				} else {
					oMatrix.FlushToDataSource();
					oMatrix.AddRow();
					oDBDSDetail.InsertRecord(oDBDSDetail.Size);
					oDBDSDetail.Offset = oMatrix.VisualRowCount - 1;
					oDBDSDetail.SetValue("LineID", oDBDSDetail.Offset, oMatrix.VisualRowCount.ToString());

					oMatrix.SetLineData(oMatrix.VisualRowCount);
					oMatrix.FlushToDataSource();
				}
			} catch (Exception ex) {
				EventHandler.oApplication.StatusBar.SetText("SetNewLine Method Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
			}
		}
        public void ChooseFromListFilterationCost(SAPbouiCOM.Form oForm, string strCFL_ID, string strCFL_Alies, string strQuery)
        {
            try
            {
                SAPbouiCOM.ChooseFromList oCFL = oForm.ChooseFromLists.Item(strCFL_ID);
                SAPbouiCOM.Conditions oConds = null;
                SAPbouiCOM.Condition oCond = null;
                SAPbouiCOM.Conditions oEmptyConds = new SAPbouiCOM.Conditions();
                SAPbobsCOM.Recordset rsetCFL = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oCFL.SetConditions(oEmptyConds);
                oConds = oCFL.GetConditions();

                rsetCFL.DoQuery(strQuery);
                rsetCFL.MoveFirst();
                for (int i = 1; i <= rsetCFL.RecordCount; i++)
                {
                    if (i == (rsetCFL.RecordCount))
                    {
                        oCond = oConds.Add();
                        oCond.Alias = strCFL_Alies;
                        oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        oCond.CondVal = "D";
                    }
                    else
                    {
                        oCond = oConds.Add();
                        oCond.Alias = strCFL_Alies;
                        oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        oCond.CondVal = "D";
                        oCond.Relationship = SAPbouiCOM.BoConditionRelationship.cr_OR;
                    }
                    rsetCFL.MoveNext();
                }
                if (rsetCFL.RecordCount == 0)
                {
                    oCond = oConds.Add();
                    oCond.Alias = strCFL_Alies;
                    oCond.Relationship = SAPbouiCOM.BoConditionRelationship.cr_NONE;
                    oCond.CondVal = "-1";
                }
                oCFL.SetConditions(oConds);
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("Choose FromList Filter Global Fun. Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            finally
            {
            }
        }
        public void ChooseFromListFilterationSales(SAPbouiCOM.Form oForm, string strCFL_ID, string strCFL_Alies, string strQuery)
        {
            try
            {
                SAPbouiCOM.ChooseFromList oCFL = oForm.ChooseFromLists.Item(strCFL_ID);
                SAPbouiCOM.Conditions oConds = null;
                SAPbouiCOM.Condition oCond = null;
                SAPbouiCOM.Conditions oEmptyConds = new SAPbouiCOM.Conditions();
                SAPbobsCOM.Recordset rsetCFL = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                oCFL.SetConditions(oEmptyConds);
                oConds = oCFL.GetConditions();

                rsetCFL.DoQuery(strQuery);
                rsetCFL.MoveFirst();
                for (int i = 1; i <= rsetCFL.RecordCount; i++)
                {
                    if (i == (rsetCFL.RecordCount))
                    {
                        oCond = oConds.Add();
                        oCond.Alias = strCFL_Alies;
                        oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        oCond.CondVal = "A";
                    }
                    else
                    {
                        oCond = oConds.Add();
                        oCond.Alias = strCFL_Alies;
                        oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        oCond.CondVal = "A";
                        oCond.Relationship = SAPbouiCOM.BoConditionRelationship.cr_OR;
                    }
                    rsetCFL.MoveNext();
                }
                if (rsetCFL.RecordCount == 0)
                {
                    oCond = oConds.Add();
                    oCond.Alias = strCFL_Alies;
                    oCond.Relationship = SAPbouiCOM.BoConditionRelationship.cr_NONE;
                    oCond.CondVal = "-1";
                }
                oCFL.SetConditions(oConds);
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("Choose FromList Filter Global Fun. Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            finally
            {
            }
        }
        public void ChooseFromListFilteration(SAPbouiCOM.Form oForm, string strCFL_ID, string strCFL_Alies, string strQuery)
		{
			try {
				SAPbouiCOM.ChooseFromList oCFL = oForm.ChooseFromLists.Item(strCFL_ID);
				SAPbouiCOM.Conditions oConds = null;
				SAPbouiCOM.Condition oCond = null;
				SAPbouiCOM.Conditions oEmptyConds = new SAPbouiCOM.Conditions();
				SAPbobsCOM.Recordset rsetCFL = GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
				oCFL.SetConditions(oEmptyConds);
				oConds = oCFL.GetConditions();

				rsetCFL.DoQuery(strQuery);
				rsetCFL.MoveFirst();
				for (int i = 1; i <= rsetCFL.RecordCount; i++) {
					if (i == (rsetCFL.RecordCount)) {
						oCond = oConds.Add();
						oCond.Alias = strCFL_Alies;
						oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        string oValue =Convert.ToString(rsetCFL.Fields.Item(0).Value);
                        oCond.CondVal = Convert.ToString(rsetCFL.Fields.Item(0).Value);
					} else {
						oCond = oConds.Add();
						oCond.Alias = strCFL_Alies;
						oCond.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
						oCond.CondVal = Convert.ToString(rsetCFL.Fields.Item(0).Value);
						oCond.Relationship = SAPbouiCOM.BoConditionRelationship.cr_OR;
					}
					rsetCFL.MoveNext();
				}
				if (rsetCFL.RecordCount == 0) {
					oCond = oConds.Add();
					oCond.Alias = strCFL_Alies;
					oCond.Relationship = SAPbouiCOM.BoConditionRelationship.cr_NONE;
					oCond.CondVal = "-1";
				}
				oCFL.SetConditions(oConds);
			} catch (Exception ex) {
				EventHandler.oApplication.StatusBar.SetText("Choose FromList Filter Global Fun. Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
			} finally {
			}
		}
		public void DeleteEmptyRowInFormDataEvent(SAPbouiCOM.Matrix oMatrix, string ColumnUID, SAPbouiCOM.DBDataSource oDBDSDetail)
		{
			try {
				if (oMatrix.VisualRowCount > 1) {
					int rows = oMatrix.RowCount;
					for (int i = 1; i <= rows - 1; i++) {
						if (oMatrix.Columns.Item(ColumnUID).Cells.Item(i).Specific.Value.Equals("")) {
							oMatrix.DeleteRow(i);
							oDBDSDetail.RemoveRecord(i - 1);
							oMatrix.FlushToDataSource();
							// rows -= 1
						}
					}
					if (oMatrix.Columns.Item(ColumnUID).Cells.Item(oMatrix.RowCount).Specific.Value.Equals("")) {
						oMatrix.DeleteRow(oMatrix.RowCount);
						oDBDSDetail.RemoveRecord(oMatrix.RowCount - 1);
						oMatrix.FlushToDataSource();
						// rows -= 1
					}
				} else if (oMatrix.VisualRowCount == 0) {
					GlobalVariables.oGFun.SetNewLine(oMatrix, oDBDSDetail);
				}
			} catch (Exception ex) {
				EventHandler.oApplication.StatusBar.SetText("Delete Empty RowIn Function Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
			} finally {
			}
		}
        public void DeleteEmptyRowInFormDataEventExportInvoice(SAPbouiCOM.Matrix oMatrix, string ColumnUID, string ColumnUID1, SAPbouiCOM.DBDataSource oDBDSDetail)
        {
            try
            {
                if (oMatrix.VisualRowCount > 1)
                {
                    int rows = oMatrix.RowCount;
                    for (int i = 1; i <= rows - 1; i++)
                    {
                        if (oMatrix.Columns.Item(ColumnUID).Cells.Item(i).Specific.Value.Equals("") && oMatrix.Columns.Item(ColumnUID1).Cells.Item(i).Specific.Value.Equals(""))
                        {
                            oMatrix.DeleteRow(i);
                            oDBDSDetail.RemoveRecord(i - 1);
                            oMatrix.FlushToDataSource();
                            // rows -= 1
                        }
                    }
                    if (oMatrix.Columns.Item(ColumnUID).Cells.Item(oMatrix.RowCount).Specific.Value.Equals("") && oMatrix.Columns.Item(ColumnUID1).Cells.Item(oMatrix.RowCount).Specific.Value.Equals(""))
                    {
                        oMatrix.DeleteRow(oMatrix.RowCount);
                        oDBDSDetail.RemoveRecord(oMatrix.RowCount - 1);
                        oMatrix.FlushToDataSource();
                        // rows -= 1
                    }
                }
                else if (oMatrix.VisualRowCount == 0)
                {
                    GlobalVariables.oGFun.SetNewLine(oMatrix, oDBDSDetail);
                }
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("Delete Empty RowIn Function Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            finally
            {
            }
        }
        public void StatusBarErrorMsg(string ErrorMsg)
		{
			try {
				EventHandler.oApplication.StatusBar.SetText(ErrorMsg, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
			} catch (Exception ex) {
				EventHandler.oApplication.StatusBar.SetText("StatusBar ErrorMsg Method Failed" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
			} finally {
			}
		}
        public void InsertDate(SAPbouiCOM.EditText date, DateTime DateTimeValue)
        {
            try
            {
                if (DateTimeValue > new DateTime(1920, 1, 1))
                {
                    date.Value = Convert.ToDateTime(DateTimeValue).ToString("yyyyMMdd");
                }
                else
                {
                    date.Value = "";
                }
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
        }
        #endregion

        #region "       Attachment Functions     "

      
        public void ShowFolderBrowserpdf()
        {
            System.Diagnostics.Process[] MyProcs = null;
            GlobalVariables.BankFileName = "";
            GlobalVariables.FileName = "";
            OpenFileDialog OpenFile = new OpenFileDialog();
            try
            {
                OpenFile.Multiselect = false;
                OpenFile.Filter = "PDF(*.)|*.pdf";
                //   "|*.*"

                int filterindex = 0;
                try
                {
                    filterindex = 0;
                }
                catch (Exception ex)
                {
                }
                OpenFile.FilterIndex = filterindex;
                OpenFile.RestoreDirectory = true;
                //Dim sPath As String = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                //If sPath.Equals("") = False Then OpenFile.InitialDirectory = sPath

                MyProcs = Process.GetProcessesByName("SAP Business One");
                if (MyProcs.Length > 0)
                {
                    for (int i = 0; i <= MyProcs.Length - 1; i++)
                    {
                        WindowWrapper MyWindow = new WindowWrapper(MyProcs[i].MainWindowHandle);
                        DialogResult ret = OpenFile.ShowDialog(MyWindow);
                        string initialdirectory = OpenFile.InitialDirectory;
                        if (ret == DialogResult.OK)
                        {
                            GlobalVariables.BankFileName = OpenFile.FileName;
                            GlobalVariables.FileName = OpenFile.SafeFileName;
                            OpenFile.Dispose();
                            break; // TODO: might not be correct. Was : Exit Try
                        }
                        else
                        {
                            System.Windows.Forms.Application.ExitThread();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText(ex.Message);
                GlobalVariables.BankFileName = "";
            }
            finally
            {
                OpenFile.Dispose();
            }
        }

        public bool FileOpen(string path)
        {
            System.IO.FileStream fs = null;
            bool fileInUse = true;
            try
            {
                fs = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
                fs.Close();

                return true;
            }
            catch (Exception ex)
            {
                GlobalVariables.oGFun.StatusBarErrorMsg("This " + path + " already opened. Close the file before importing");
                return false;
            }
        }


        public string FindFile()
        {
            System.Threading.Thread ShowFolderBrowserThread = null;
            try
            {
                ShowFolderBrowserThread = new System.Threading.Thread(ShowFolderBrowser);
                if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Unstarted)
                {
                    ShowFolderBrowserThread.SetApartmentState(System.Threading.ApartmentState.STA);
                    ShowFolderBrowserThread.Start();
                }
                else if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    ShowFolderBrowserThread.Start();
                    ShowFolderBrowserThread.Join();
                }
                while (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Running)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
                if (!string.IsNullOrEmpty(GlobalVariables.BankFileName))
                {
                    return GlobalVariables.BankFileName + "," + GlobalVariables.FileName;
                }
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.MessageBox("FileFile Method Failed : " + ex.Message);
            }
            return "";
        }
        public string FindpdfFile()
        {
            System.Threading.Thread ShowFolderBrowserThread = null;
            try
            {
                ShowFolderBrowserThread = new System.Threading.Thread(ShowFolderBrowserpdf);
                if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Unstarted)
                {
                    ShowFolderBrowserThread.SetApartmentState(System.Threading.ApartmentState.STA);
                    ShowFolderBrowserThread.Start();
                }
                else if (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    ShowFolderBrowserThread.Start();
                    ShowFolderBrowserThread.Join();
                }
                while (ShowFolderBrowserThread.ThreadState == System.Threading.ThreadState.Running)
                {
                    System.Windows.Forms.Application.DoEvents();
                }
                if (!string.IsNullOrEmpty(GlobalVariables.BankFileName))
                {
                    return GlobalVariables.BankFileName + "," + GlobalVariables.FileName;
                }
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.MessageBox("FileFile Method Failed : " + ex.Message);
            }
            return "";
        }

        public void OpenFile(string ServerPath, string ClientPath)
        {
            try
            {
                System.Diagnostics.Process oProcess = new System.Diagnostics.Process();
                try
                {
                    oProcess.StartInfo.FileName = ServerPath;
                    oProcess.Start();
                }
                catch (Exception ex1)
                {
                    try
                    {
                        oProcess.StartInfo.FileName = ClientPath;
                        oProcess.Start();
                    }
                    catch (Exception ex2)
                    {
                        EventHandler.oApplication.StatusBar.SetText("" + ex2.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                    }
                    finally
                    {
                    }
                }
                finally
                {
                }
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            finally
            {
            }
        }


        public class WindowWrapper : System.Windows.Forms.IWin32Window
        {

            private IntPtr _hwnd;
            public WindowWrapper(IntPtr handle)
            {
                _hwnd = handle;
            }

            public System.IntPtr Handle
            {
                get { return _hwnd; }
            }

        }



        #endregion

        #region "          Attachment Option          "

        public void AddAttachment(SAPbouiCOM.Matrix oMatAttach, SAPbouiCOM.DBDataSource oDBDSAttch, SAPbouiCOM.DBDataSource oDBDSHeader)
        {
            try
            {
                if (oMatAttach.VisualRowCount > 0)
                {
                    SAPbobsCOM.Recordset rsetAttCount = (SAPbobsCOM.Recordset)GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    SAPbobsCOM.Attachments2 oAttachment = (SAPbobsCOM.Attachments2)GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oAttachments2);
                    SAPbobsCOM.Attachments2_Lines oAttchLines = null;
                    oAttchLines = oAttachment.Lines;
                    oMatAttach.FlushToDataSource();
                    rsetAttCount.DoQuery("Select Count(*) From ATC1 Where AbsEntry = '" + Strings.Trim(oDBDSHeader.GetValue("U_AtcEntry", 0)) + "'");

                    if (Strings.Trim(Convert.ToString(rsetAttCount.Fields.Item(0).Value)).Equals("0"))
                    {
                        for (int i = 1; i <= oMatAttach.VisualRowCount; i++)
                        {
                            if (i > 1)
                                oAttchLines.Add();
                            oDBDSAttch.Offset = i - 1;
                            oAttchLines.SourcePath = Strings.Trim(oDBDSAttch.GetValue("U_ScrPath", oDBDSAttch.Offset));
                            oAttchLines.FileName = Strings.Trim(oDBDSAttch.GetValue("U_FileName", oDBDSAttch.Offset));
                            oAttchLines.FileExtension = Strings.Trim(oDBDSAttch.GetValue("U_FileExt", oDBDSAttch.Offset));
                            oAttchLines.Override = SAPbobsCOM.BoYesNoEnum.tYES;
                        }
                        oAttachment.Add();
                        SAPbobsCOM.Recordset rsetAttch = (SAPbobsCOM.Recordset)GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        rsetAttch.DoQuery("Select  Case When Count(*) > 0 Then  Max(AbsEntry) Else 0 End AbsEntry  From ATC1");
                        oDBDSHeader.SetValue("U_AtcEntry", 0, Convert.ToString(rsetAttch.Fields.Item(0).Value));
                    }
                    else
                    {
                        oAttachment.GetByKey(Convert.ToInt32(Strings.Trim(oDBDSHeader.GetValue("U_AtcEntry", 0))));
                        for (int i = 1; i <= oMatAttach.VisualRowCount; i++)
                        {
                            if (oAttchLines.Count < i)
                                oAttchLines.Add();
                            oDBDSAttch.Offset = i - 1;
                            oAttchLines.SetCurrentLine(i - 1);
                            oAttchLines.SourcePath = Strings.Trim(oDBDSAttch.GetValue("U_ScrPath", oDBDSAttch.Offset));
                            oAttchLines.FileName = Strings.Trim(oDBDSAttch.GetValue("U_FileName", oDBDSAttch.Offset));
                            oAttchLines.FileExtension = Strings.Trim(oDBDSAttch.GetValue("U_FileExt", oDBDSAttch.Offset));
                            oAttchLines.Override = SAPbobsCOM.BoYesNoEnum.tYES;
                        }
                        oAttachment.Update();
                    }
                }
                //Delete the Attachment Rows...
                SAPbobsCOM.Recordset rsetDelete = (SAPbobsCOM.Recordset)GlobalVariables.oCompany.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                rsetDelete.DoQuery("Delete From ATC1 Where AbsEntry = '" + Strings.Trim(oDBDSHeader.GetValue("U_AtcEntry", 0)) + "' And Line >'" + oMatAttach.VisualRowCount + "' ");

            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("AddAttachment Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            finally
            {
            }
        }

        public void DeleteRowAttachment(SAPbouiCOM.Form oForm, SAPbouiCOM.Matrix oMatrix, SAPbouiCOM.DBDataSource oDBDSAttch, int SelectedRowID)
        {
            try
            {
                string oFile = Strings.Trim(((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_2").Cells.Item(SelectedRowID).Specific).Value) +"\\"+ Strings.Trim(((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_1").Cells.Item(SelectedRowID).Specific).Value);
                if (!string.IsNullOrEmpty(oFile))
                {
                    File.Delete(oFile);
                }
                oDBDSAttch.RemoveRecord(SelectedRowID - 1);
                oMatrix.DeleteRow(SelectedRowID);
                oMatrix.FlushToDataSource();
                
                for (int i = 1; i <= oMatrix.VisualRowCount; i++)
                {
                    oMatrix.GetLineData(i);
                    oDBDSAttch.Offset = i - 1;
                  
                    oDBDSAttch.SetValue("LineId", oDBDSAttch.Offset, i.ToString());
                    oDBDSAttch.SetValue("U_TrgtPath", oDBDSAttch.Offset, Strings.Trim(((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_2").Cells.Item(i).Specific).Value));
                  //  oDBDSAttch.SetValue("U_ScrPath", oDBDSAttch.Offset, Strings.Trim(((SAPbouiCOM.EditText)oMatrix.Columns.Item("scrpath").Cells.Item(i).Specific).Value));
                    oDBDSAttch.SetValue("U_FileName", oDBDSAttch.Offset, Strings.Trim(((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_1").Cells.Item(i).Specific).Value));
                  // oDBDSAttch.SetValue("U_FileExt", oDBDSAttch.Offset, Strings.Trim(((SAPbouiCOM.EditText)oMatrix.Columns.Item("fileext").Cells.Item(i).Specific).Value));
                    oDBDSAttch.SetValue("U_Date", oDBDSAttch.Offset, Strings.Trim(((SAPbouiCOM.EditText)oMatrix.Columns.Item("V_0").Cells.Item(i).Specific).Value));
                    oMatrix.SetLineData(i);
                    oMatrix.FlushToDataSource();
                }
                //oDBDSAttch.RemoveRecord(oDBDSAttch.Size - 1)
                oMatrix.LoadFromDataSource();

                //oForm.Items.Item("b_display").Enabled = false;
                //oForm.Items.Item("b_delete").Enabled = false;

                if (oForm.Mode != SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE;

            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("DeleteRowAttachment Method Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            finally
            {
            }
        }

        public bool SetAttachMentFile(SAPbouiCOM.Form oForm, SAPbouiCOM.DBDataSource oDBDSHeader, SAPbouiCOM.Matrix oMatrix, SAPbouiCOM.DBDataSource oDBDSAttch)
        {
            try
            {
                if (GlobalVariables.oCompany.AttachMentPath.Length <= 0)
                {
                    GlobalVariables.oGFun.StatusBarErrorMsg("Attchment folder not defined, or Attchment folder has been changed or removed. [Message 131-102]");
                    return false;
                }

                string strFileName = FindFile();
                if (strFileName.Equals("") == false)
                {
                    string[] FileExist = strFileName.Split('\\');
                    string FileDestPath = GlobalVariables.oCompany.AttachMentPath + FileExist[FileExist.Length - 1];

                    if (File.Exists(FileDestPath))
                    {
                        long LngRetVal = EventHandler.oApplication.MessageBox("A file with this name already exists,would you like to replace this?  " + FileDestPath + " will be replaced.", 1, "Yes", "No");
                        if (LngRetVal != 1)
                            return false;
                    }
                    string[] fileNameExt = FileExist[FileExist.Length - 1].Split('.');
                    string ScrPath = GlobalVariables.oCompany.AttachMentPath;
                    ScrPath = ScrPath.Substring(0, ScrPath.Length - 1);
                    string TrgtPath = strFileName.Substring(0, strFileName.LastIndexOf("\\"));
                    try
                    {
                        string oSource = TrgtPath +"\\"+ fileNameExt[0]+ "."+fileNameExt[2];
                        string otatrget = ScrPath + "\\"+ fileNameExt[0] + "." + fileNameExt[2];
                        // Copy the file
                        File.Copy(oSource, otatrget);
                    oMatrix.AddRow();
                    oMatrix.FlushToDataSource();
                    oDBDSAttch.Offset = oDBDSAttch.Size - 1;
                    oDBDSAttch.SetValue("LineID", oDBDSAttch.Offset, oMatrix.VisualRowCount.ToString());
                    oDBDSAttch.SetValue("U_TrgtPath", oDBDSAttch.Offset, ScrPath);
                    oDBDSAttch.SetValue("U_ScrPath", oDBDSAttch.Offset, TrgtPath);
                    oDBDSAttch.SetValue("U_FileName", oDBDSAttch.Offset, fileNameExt[0]+"."+ fileNameExt[2]);
                    oDBDSAttch.SetValue("U_FileExt", oDBDSAttch.Offset, fileNameExt[1]);
                    oDBDSAttch.SetValue("U_Date", oDBDSAttch.Offset, GlobalVariables.oGFun.GetServerDate());
                    oMatrix.SetLineData(oDBDSAttch.Size);
                    oMatrix.FlushToDataSource();
                        Console.WriteLine("File copied successfully.");
                    }
                    catch (IOException e)
                    {
                        EventHandler.oApplication.StatusBar.SetText(e.Message);
                        return false;
                    }
                    if (oForm.Mode != SAPbouiCOM.BoFormMode.fm_ADD_MODE)
                        oForm.Mode = SAPbouiCOM.BoFormMode.fm_UPDATE_MODE;
                }
                return true;
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("Set AttachMent File Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                return false;
            }
            finally
            {
            }
        }

        public void OpenAttachment(SAPbouiCOM.Matrix oMatrix, SAPbouiCOM.DBDataSource oDBDSAttch, int PvalRow)
        {
            try
            {
                if (PvalRow <= oMatrix.VisualRowCount & PvalRow != 0)
                {
                    int RowIndex = oMatrix.GetNextSelectedRow(0, SAPbouiCOM.BoOrderType.ot_RowOrder) - 1;
                    string strServerPath = null;
                    string strClientPath = null;

                    strServerPath = Strings.Trim(oDBDSAttch.GetValue("U_TrgtPath", PvalRow-1)) + "\\" + Strings.Trim(oDBDSAttch.GetValue("U_FileName", PvalRow - 1));
                    strClientPath = Strings.Trim(oDBDSAttch.GetValue("U_ScrPath", PvalRow-1)) + "\\" + Strings.Trim(oDBDSAttch.GetValue("U_FileName", PvalRow - 1)) ;
                    //Open Attachment File
                    this.OpenFile(strServerPath, strClientPath);
                }

            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText("OpenAttachment Method Failed:" + ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
            }
            finally
            {
            }
        }

        public void AttchButtonEnable(SAPbouiCOM.Form oForm, SAPbouiCOM.Matrix Matrix, int PvalRow)
        {
            try
            {
                if (PvalRow <= Matrix.VisualRowCount & PvalRow != 0)
                {
                    Matrix.SelectRow(PvalRow, true, false);
                    if (Matrix.IsRowSelected(PvalRow) == true)
                    {
                        oForm.Items.Item("b_display").Enabled = true;
                        oForm.Items.Item("b_delete").Enabled = true;
                    }
                    else
                    {
                        oForm.Items.Item("b_display").Enabled = false;
                        oForm.Items.Item("b_delete").Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                GlobalVariables.oGFun.StatusBarErrorMsg("Attach Button Enble Function...");
            }
        }

        #endregion


        public bool GetCheckboxValue(SAPbouiCOM.Matrix aMatrix, string colUid, int intRow)
        {
            var checkbox = (SAPbouiCOM.CheckBox)aMatrix.Columns.Item(colUid).Cells.Item(intRow).Specific;
            return checkbox.Checked;
        }




    }
}

