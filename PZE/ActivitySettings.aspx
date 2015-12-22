<%--    Copyright © 2015 by Commend International GmbH. All rights reserved.

    This program is free software: you can redistribute it and/or  modify
    it under the terms of the GNU Affero General Public License, version 3,
    as published by the Free Software Foundation.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.--%>

<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="com.commend.tools.PZE._ActivitySettings" Codebehind="ActivitySettings.aspx.cs" %>

<%@ Register Assembly="DevExpress.Web.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Xpo.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Xpo" TagPrefix="dx" %>


<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">

 <script src="Scripts/shortcut.js" type="text/javascript"></script>
    <script type="text/javascript">

        shortcut.add("CTRL+E", function () {
            activityGridview.StartEditRow(activityGridview.GetFocusedRowIndex());
        });

        shortcut.add("CTRL+N", function () {
            activityGridview.AddNewRow();
        });

        shortcut.add("delete", function () {

            if (confirm("Wollen Sie diese Tätigkeit wirklich löschen?")) {
                if (activityGridview.GetFocusedRowIndex() > -1) {
                    activityGridview.GetRowValues(activityGridview.GetFocusedRowIndex(), 'Oid', OnGetRowValues);
                }
            }
        });

        shortcut.add("CTRL+S", function () {
            activityGridview.UpdateEdit();
        });

        shortcut.add("CTRL+C", function () {
            activityGridview.CancelEdit();
        });

        shortcut.add("esc", function () {
            activityGridview.CancelEdit();
        });

        shortcut.add("CTRL+Right", function () {
            activityGridview.NextPage();
        });

        shortcut.add("CTRL+Left", function () {
            activityGridview.PrevPage();
        });


        function CustomButtonClicked() {
            if (confirm("Wollen Sie diese Tätigkeit wirklich löschen?")) {
                if (activityGridview.GetFocusedRowIndex() > -1) {
                    activityGridview.GetRowValues(activityGridview.GetFocusedRowIndex(), 'Oid', OnGetRowValues);
                }
            }
        }

        function OnGetRowValues(values) {
            PageMethods.hasActivityRecords(values, hasRecords);
        }

        function hasRecords(result)
        {
            if (result == true) {
                alert("Diese Tätigkeit ist bereits mit Einträgen verknüpft und kann nicht gelöscht werden!");
            }
            else {
                activityGridview.DeleteRow(activityGridview.GetFocusedRowIndex());
            }
        }

    </script>

    <fieldset>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True" />
        <legend>Tätigkeiten</legend>
        <dx:ASPxGridView ID="ActivityGridview" ClientInstanceName="activityGridview" ClientIDMode="Static" runat="server" DataSourceID="ActivitiesXpoData" EnableViewState="false"
            align="center" KeyFieldName="Oid" KeyboardSupport="true" Width="100%" AutoGenerateColumns="false" OnParseValue="grid_ParseValue" OnStartRowEditing="grid_StartRowEditing"  OnRowValidating="grid_RowValidating">
            <SettingsCommandButton>
                 <EditButton ButtonType="Image">
                     <Image Url="~/Icons/edit.png" ToolTip="edit" />
                 </EditButton>
                 <NewButton ButtonType="Image">
                    <Image Url="~/Icons/add.png" ToolTip="new" />
                 </NewButton>
                 <UpdateButton ButtonType="Image">
                     <Image ToolTip="Save changes and close Edit Form" Url="~/Icons/update.png" />
                 </UpdateButton>
                 <CancelButton ButtonType="Image">
                     <Image ToolTip="Close Edit Form without saving changes" Url="~/Icons/cancel.png" />
                 </CancelButton>
             </SettingsCommandButton>
            <Columns>
                <dx:GridViewCommandColumn VisibleIndex="0" Width="50px" Caption=" " ShowClearFilterButton="true" ShowEditButton="true" ShowNewButtonInHeader="True" ButtonType="Image">
                    <CustomButtons>
                        <dx:GridViewCommandColumnCustomButton ID="del" Image-Url="~/Icons/delete.png" Image-AlternateText="delete" Image-ToolTip="delete">
                         </dx:GridViewCommandColumnCustomButton>
                    </CustomButtons>
                </dx:GridViewCommandColumn>
                <dx:GridViewDataTextColumn FieldName="Oid" Caption="UserId" Visible="false">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn VisibleIndex="1" FieldName="ActivityNumber" Caption="TätigkeitsNummer"
                    EditFormSettings-VisibleIndex="0" Width="100">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn VisibleIndex="2" FieldName="ActivityName" Caption="Tätigkeit"
                    EditFormSettings-VisibleIndex="1">
                </dx:GridViewDataTextColumn>
            </Columns>
            <SettingsEditing PopupEditFormWidth="600" PopupEditFormModal="true" NewItemRowPosition="Bottom" />
            <Templates>
                <EditForm>
                    <div style="padding: 4px 4px 3px 4px">
                        <dx:ASPxPageControl runat="server" ID="pageControl" Width="100%">
                            <TabPages>
                                <dx:TabPage Text="Edit" Visible="true">
                                    <ContentCollection>
                                        <dx:ContentControl ID="ContentControl1" runat="server">
                                            <dx:ASPxGridViewTemplateReplacement ID="Editors" ReplacementType="EditFormEditors"
                                                runat="server"></dx:ASPxGridViewTemplateReplacement>
                                        </dx:ContentControl>
                                    </ContentCollection>
                                </dx:TabPage>
                            </TabPages>
                        </dx:ASPxPageControl>
                    </div>
                    <div style="text-align: right; padding: 2px 2px 2px 2px">
                        <dx:ASPxGridViewTemplateReplacement ID="UpdateButton" ReplacementType="EditFormUpdateButton"
                            runat="server"></dx:ASPxGridViewTemplateReplacement>
                        <dx:ASPxGridViewTemplateReplacement ID="CancelButton" ReplacementType="EditFormCancelButton"
                            runat="server"></dx:ASPxGridViewTemplateReplacement>
                    </div>
                </EditForm>
            </Templates>
           <ClientSideEvents CustomButtonClick="CustomButtonClicked"/>
            <Styles AlternatingRow-Enabled="True" FocusedRow-BackColor="#0077C0" />
            <SettingsEditing PopupEditFormWidth="600" PopupEditFormModal="true" NewItemRowPosition="Bottom" />
            <SettingsText  CommandUpdate="Save" CommandClearFilter="clear Filter" />
            <Settings ShowFilterRow="True" EnableFilterControlPopupMenuScrolling="true"
                ShowFooter="True" ShowGroupFooter="VisibleIfExpanded" />
            <SettingsPager PageSize="20" Visible="true" />
        </dx:ASPxGridView>
    </fieldset>
    <dx:XpoDataSource ID="ActivitiesXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Activities">
    </dx:XpoDataSource>
</asp:Content>
