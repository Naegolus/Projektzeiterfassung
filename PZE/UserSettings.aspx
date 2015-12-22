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

<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="com.commend.tools.PZE._UserSettings" Codebehind="UserSettings.aspx.cs" %>

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
            userGridview.StartEditRow(userGridview.GetFocusedRowIndex());
        });

        shortcut.add("CTRL+N", function () {
            userGridview.AddNewRow();
        });

        shortcut.add("delete", function () {
            if (confirm("Wollen Sie diesen Benutzer wirklich löschen?")) {
                if (userGridview.GetFocusedRowIndex() > -1) {
                    userGridview.GetRowValues(userGridview.GetFocusedRowIndex(), 'Oid', OnGetRowValues);
                }
            }
        });

        shortcut.add("CTRL+S", function () {
            userGridview.UpdateEdit();
        });

        shortcut.add("CTRL+C", function () {
            userGridview.CancelEdit();
        });

        shortcut.add("esc", function () {
            userGridview.CancelEdit();
        });

        shortcut.add("CTRL+Right", function () {
            userGridview.NextPage();
        });

        shortcut.add("CTRL+Left", function () {
            userGridview.PrevPage();
        });


        function CustomButtonClicked() {
            if (confirm("Wollen Sie diesen Benutzer wirklich löschen?")) {
                if (userGridview.GetFocusedRowIndex() > -1) {
                    userGridview.GetRowValues(userGridview.GetFocusedRowIndex(), 'Oid', OnGetRowValues);
                }
            }
        }

        function OnGetRowValues(values) {
            PageMethods.hasUserRecords(values, hasRecords);
        }

        function hasRecords(result)
        {
            if (result == true) {
                alert("Dieser Benutzer hat bereits Records angelegt und kann daher nicht gelöscht werden!");
            }
            else {
                userGridview.DeleteRow(userGridview.GetFocusedRowIndex());
            }
        }
    </script>

    <fieldset>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True" />
        <legend>
            Benutzer
        </legend>
        <table>
            <tr>
                <td>
                    <dx:ASPxButton runat="server" ID="ExportButton" Text="" AutoPostBack="false" Width="30px"
                        Border-BorderColor="Transparent" BackColor="Transparent" EnableDefaultAppearance="false"
                        Cursor="pointer" UseSubmitBehavior="false" Visible="true" AllowFocus="true" Image-Url="Icons/export.png"
                        OnClick="OnExportButton_Click">
                    </dx:ASPxButton>
                </td>
                <td>
                    <dx:ASPxComboBox runat="server" ID="CmbExport" ClientInstanceName="cmbExport" ClientIDMode="Static"
                        DropDownStyle="DropDownList" IncrementalFilteringMode="Contains" AutoPostBack="false"
                        Width="100px">
                        <Items>
                            <dx:ListEditItem Text="Pdf" Value="Pdf"></dx:ListEditItem>
                            <dx:ListEditItem Text="Xls" Value="Xls" Selected="true"></dx:ListEditItem>
                            <dx:ListEditItem Text="Xlsx" Value="Xlsx"></dx:ListEditItem>
                            <dx:ListEditItem Text="Rtf" Value="Rtf"></dx:ListEditItem>
                            <dx:ListEditItem Text="Csv" Value="Csv"></dx:ListEditItem>
                        </Items>
                    </dx:ASPxComboBox>
                </td>
            </tr>
        </table>
        <dx:ASPxGridView ID="UserGridview" ClientInstanceName="userGridview" ClientIDMode="Static"
            runat="server" DataSourceID="UserXpoData" align="center" KeyFieldName="Oid" KeyboardSupport="true" Width="100%" OnRowValidating="grid_RowValidating"
            OnStartRowEditing="grid_StartRowEditing" OnParseValue="grid_ParseValue" EnableTheming="True" Theme="Default">
            <SettingsCommandButton>
                <EditButton ButtonType="Image">
                    <Image Url="~/Icons/edit.png" ToolTip="edit" />
                </EditButton>
                <NewButton ButtonType="Image">
                    <Image Url="~/Icons/add.png" ToolTip="new" />
                </NewButton>
                <UpdateButton ButtonType="Image">
                    <Image ToolTip="Save changes and close Edit Form" Url="Icons/update.png" />
                </UpdateButton>
                <CancelButton ButtonType="Image">
                    <Image ToolTip="Close Edit Form without saving changes" Url="Icons/cancel.png" />
                </CancelButton>
            </SettingsCommandButton>
            <Columns>
                <dx:GridViewCommandColumn VisibleIndex="0" Width="50px" ShowClearFilterButton="true" ShowEditButton="true" ShowNewButtonInHeader="True" ButtonType="Image">
                    <CustomButtons>
                        <dx:GridViewCommandColumnCustomButton Text=" " ID="del" Image-Url="~/Icons/delete.png">
                            <Image Url="~/Icons/delete.png" AlternateText="löschen" ToolTip="löschen"></Image>
                        </dx:GridViewCommandColumnCustomButton>
                    </CustomButtons>
                </dx:GridViewCommandColumn>
                <dx:GridViewDataTextColumn FieldName="Oid" ReadOnly="True" Visible="False" VisibleIndex="1">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn Caption="Vorname" FieldName="ForName" VisibleIndex="2">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn Caption="Nachname" FieldName="SurName" VisibleIndex="3">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn Caption="Benutzername" FieldName="UserName" VisibleIndex="4">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataComboBoxColumn Caption="Abteilung" FieldName="Division!Key" VisibleIndex="5">
                    <PropertiesComboBox DataSourceID="DivisionXpoData" ValueField="Oid">
                        <Columns>
                            <dx:ListBoxColumn Caption="Abteilung" FieldName="DivisionName" />
                        </Columns>
                    </PropertiesComboBox>
                </dx:GridViewDataComboBoxColumn>
                <dx:GridViewDataTextColumn Caption="Personal Nr." FieldName="PersonalId" VisibleIndex="6">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataComboBoxColumn Caption="Berechtigung" FieldName="Permission!Key" VisibleIndex="7">
                    <PropertiesComboBox DataSourceID="PermissionsXpoData" ValueField="Oid">
                        <Columns>
                            <dx:ListBoxColumn Caption="Berechtigung" FieldName="PermissionName" />
                        </Columns>
                    </PropertiesComboBox>
                </dx:GridViewDataComboBoxColumn>
                <dx:GridViewDataTextColumn FieldName="Password" Visible="false" EditFormSettings-Visible="true" VisibleIndex="8" PropertiesTextEdit-Password="true">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataComboBoxColumn Caption="Status" FieldName="Status!Key" VisibleIndex="9">
                    <PropertiesComboBox DataSourceID="StatusXpoData" ValueField="Oid">
                        <Columns>
                            <dx:ListBoxColumn Caption="Status" FieldName="StatusName" />
                        </Columns>
                    </PropertiesComboBox>
                </dx:GridViewDataComboBoxColumn>
                <dx:GridViewDataCheckColumn Caption="Externer Mitarbeiter" FieldName="ExternalStaff" VisibleIndex="10">
                </dx:GridViewDataCheckColumn>
            </Columns>

            <Templates>
                <EditForm>
                    <div style="padding: 4px 4px 3px 4px">
                        <dx:ASPxPageControl runat="server" ID="pageControl" Width="100%"> 
                            <TabPages>
                                <dx:TabPage Text="Data" Visible="true">
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

            <Styles AlternatingRow-Enabled="True" FocusedRow-BackColor="#0077C0" />
            <SettingsEditing PopupEditFormWidth="600" PopupEditFormModal="true" NewItemRowPosition="Bottom" />
            <SettingsText GroupPanel="Um zu gruppieren, ziehen Sie bitte die entsprechende Spalte hier her"
                CommandUpdate="Save" CommandClearFilter="clear Filter" CustomizationWindowCaption="Versteckte Spalten"/>
            <Settings ShowFilterRow="True" EnableFilterControlPopupMenuScrolling="true" ShowGroupPanel="True"
                ShowFooter="True" ShowGroupFooter="VisibleIfExpanded"/> 
             <SettingsPager PageSize="20" Visible="true" />
             <SettingsCustomizationWindow Enabled="True"/>
            <ClientSideEvents CustomButtonClick="CustomButtonClicked"/>
        </dx:ASPxGridView>
    </fieldset>
    <dx:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="UserGridview"
        PaperKind="A4" MaxColumnWidth="200">
        <Styles>
            <Cell Font-Size="Small">
            </Cell>
        </Styles>
    </dx:ASPxGridViewExporter>
    <dx:XpoDataSource ID="UserXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.User">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="DivisionXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Division">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="PermissionsXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Permissions">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="StatusXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Status">
    </dx:XpoDataSource>
</asp:Content>
