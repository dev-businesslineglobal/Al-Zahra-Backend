using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
namespace GardenAPP
{

    /// <summary>
    /// GGlobally whatever variable do you want declare here 
    /// We can use any class and module from here  
    /// </summary>
    /// <remarks></remarks>
    static class GlobalVariables
    {

        #region " ... Common For SAP ..."
        public static SAPbobsCOM.Company oCompany;
        public static GlobalFunctions oGFun = new GlobalFunctions();
        public static SAPbouiCOM.Form oForm;

        #endregion

        #region " ... Common For Forms ..."
        public static string contractNo = "";

        public static string configurationID = "CONF";
        public static string confXML = "Presentation_Layer.Masters.Configuration.xml";
        public static Configuration oConfig = new Configuration();

        public static SAPbouiCOM.Form frmAP;

        #endregion

        #region " ... Gentral Purpose ..."
        public static long v_RetVal;
        public static int v_ErrCode;
        public static string v_ErrMsg = "";
        public static string addonName = "Garden APP";
        public static string sQuery = "";
        public static string BankFileName = "";
        public static string FileName = "";
        #endregion
    }
}
