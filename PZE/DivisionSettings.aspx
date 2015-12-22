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

<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="com.commend.tools.PZE._DivisionSettings" Codebehind="DivisionSettings.aspx.cs" %>

<%@ Register Assembly="DevExpress.Web.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Xpo.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Xpo" TagPrefix="dx" %>


<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <script src="Scripts/shortcut.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">

        shortcut.add("CTRL+E", function () {
            divisionGridview.StartEditRow(divisionGridview.GetFocusedRowIndex());
        });

        shortcut.add("CTRL+N", function () {
            divisionGridview.AddNewRow();
        });

        shortcut.add("delete", function () {

            if (confirm("Wollen Sie diesen Bereich wirklich löschen?")) 
            {
                if (divisionGridview.GetFocusedRowIndex() > -1) {
                    divisionGridview.GetRowValues(divisionGridview.GetFocusedRowIndex(), 'Oid', OnGetRowValues);
                }
            }
        });

        shortcut.add("CTRL+S", function () {
            divisionGridview.UpdateEdit();
        });

        shortcut.add("CTRL+C", function () {
            divisionGridview.CancelEdit();
        });

        shortcut.add("esc", function () {
            divisionGridview.CancelEdit();
        });

        shortcut.add("CTRL+Right", function () {
            divisionGridview.NextPage();
        });

        shortcut.add("CTRL+Left", function () {
            divisionGridview.PrevPage();
        });


        function CustomButtonClicked() {
            if (confirm("Wollen Sie diesen Bereich wirklich löschen?")) 
            {
                if (divisionGridview.GetFocusedRowIndex() > -1) {
                    divisionGridview.GetRowValues(divisionGridview.GetFocusedRowIndex(), 'Oid', OnGetRowValues);
                }
            }
        }

        function OnGetRowValues(values) {
            PageMethods.hasDivisionUsers(values, hasRecords);
        }

        function hasRecords(result) {
            if (result == true) {
                alert("Dieser Bereich besitzt Benutzer und kann daher nicht gelöscht werden!");
            }
            else {
                divisionGridview.DeleteRow(divisionGridview.GetFocusedRowIndex());
            }
        }

    </script>
    <fieldset>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True" />
        <legend>Bereiche</legend>

        <dx:ASPxGridView ID="DivisionGridview" ClientInstanceName="divisionGridview" ClientIDMode="Static" 
        runat="server" DataSourceID="DivisionXpoData" align="center" KeyFieldName="Oid" KeyboardSupport="true" EnableViewState="false" 
        Width="100%" AutoGenerateColumns="false" OnStartRowEditing="grid_StartRowEditing" OnRowValidating="grid_RowValidating" OnRowUpdating="Grid_RowUpdating">
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
                <dx:GridViewCommandColumn VisibleIndex="0" Width="50px" ShowEditButton="true" ShowNewButtonInHeader="True" ButtonType="Image">
                    <CustomButtons>
                        <dx:GridViewCommandColumnCustomButton Text=" " ID="del" Image-Url="~/Icons/delete.png" Image-AlternateText="delete" Image-ToolTip="delete">
                         </dx:GridViewCommandColumnCustomButton>
                    </CustomButtons>
                </dx:GridViewCommandColumn>
                <dx:GridViewDataTextColumn FieldName="Oid" Caption="DivisionID" Visible="false">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn VisibleIndex="1" FieldName="DivisionName" Caption="Bereichsname"
                    EditFormSettings-VisibleIndex="0">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataComboBoxColumn VisibleIndex="2" EditFormSettings-VisibleIndex="1"
                        FieldName="Leader!Key" Caption="Leiter" CellStyle-Wrap="False" Width="60px">
                        <PropertiesComboBox DataSourceID="UserXpoData" TextField="UserName" ValueField="Oid"
                            IncrementalFilteringMode="StartsWith" />
                        <DataItemTemplate>
                            <div style="width: 100px; overflow: hidden; white-space: nowrap;" title='<%# GetUserNameByOid(Eval("Leader!Key")) %>'>
                                <%# GetUserNameByOid(Eval("Leader!Key"))%>
                            </div>
                        </DataItemTemplate>
                        <EditItemTemplate>
                            <dx:ASPxComboBox runat="server" ID="CmbLeader" ClientInstanceName="cmbLeaderChanged"
                                ClientIDMode="Static" Width="100%" DropDownStyle="DropDownList" IncrementalFilteringMode="Contains"
                                Value='<%# Eval("Leader!Key") %>' DataSourceID="UserXpoData" TextFormatString="{0} ({1})"
                                ValueField="Oid" ValueType="System.Int32" EnableSynchronization="False">
                                <Columns>
                                    <dx:ListBoxColumn FieldName="UserName" />
                                    <dx:ListBoxColumn FieldName="ForName" />
                                </Columns>
                            </dx:ASPxComboBox>
                        </EditItemTemplate>
                    </dx:GridViewDataComboBoxColumn>
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
            <ClientSideEvents CustomButtonClick="CustomButtonClicked" />
            <Styles AlternatingRow-Enabled="True" FocusedRow-BackColor="#0077C0" />
            <SettingsEditing PopupEditFormWidth="600" PopupEditFormModal="true" NewItemRowPosition="Bottom" />
            <SettingsText CommandUpdate="Save" CommandClearFilter="clear Filter" />
            <Settings ShowFilterRow="True" EnableFilterControlPopupMenuScrolling="true" ShowFooter="True"
                ShowGroupFooter="VisibleIfExpanded" />
            <SettingsPager PageSize="20" Visible="true" />
        </dx:ASPxGridView>
    </fieldset>
    <dx:XpoDataSource ID="DivisionXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Division">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="UserXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.User">
    </dx:XpoDataSource>
</asp:Content>
