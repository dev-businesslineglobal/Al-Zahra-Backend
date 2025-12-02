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



	static class MainM
	{

        #region "... Main ..."
        static void Main(string[] args)
        {
			try {
				GlobalVariables.oGFun.SetApplication();
					if (!(GlobalVariables.oGFun.CookieConnect() == 0))
                    {   EventHandler.oApplication.MessageBox("DI Api Conection Failed");
                        System.Environment.Exit(0);
                    }
                    if (!(GlobalVariables.oGFun.ConnectionContext() == 0))
                    {
                        EventHandler.oApplication.MessageBox("Failed to Connect Company");

                        System.Environment.Exit(0);
                    }
              
			}
            catch (Exception ex) {
				System.Windows.Forms.MessageBox.Show("Application Not Found", "Garden APP Add-on" + ex.Message);
				System.Windows.Forms.Application.ExitThread();
			} finally {
			}
			try {
			
				try {
                    TableCreation oTableCreation = new TableCreation();
                    EventHandler.SetEventFilter();
					GlobalVariables.oGFun.AddXML("Presentation_Layer.Menu.xml");
                    SetMenuChecked("GR");


                }
                catch (Exception ex) {
					System.Windows.Forms.MessageBox.Show(ex.Message);
					System.Windows.Forms.Application.ExitThread();
				} finally {
				}

				EventHandler.oApplication.StatusBar.SetText("Connected.......", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
				System.Windows.Forms.Application.Run();
			
			} catch (Exception ex) {
				System.Windows.Forms.MessageBox.Show(GlobalVariables.addonName + " Main Method Failed : " + ex.Message);
				System.Windows.Forms.Application.ExitThread();

			} finally {
			}
		}
        private  static void SetMenuChecked(string menuUID)
        {
            try
            {
                SAPbouiCOM.MenuItem menu = EventHandler.oApplication.Menus.Item(menuUID);

                if (menu != null)
                {
                    // Temporarily disable and enable to refresh state
                    if (menu.Enabled)
                        menu.Enabled = false;

                    menu.Checked = true;
                    menu.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                EventHandler.oApplication.StatusBar.SetText(
                    $"Error updating menu {menuUID}: {ex.Message}",
                    SAPbouiCOM.BoMessageTime.bmt_Short,
                    SAPbouiCOM.BoStatusBarMessageType.smt_Error
                );
            }
        }
        #endregion
    }
}
