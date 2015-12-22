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

<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="com.commend.tools.PZE._PspCodeSettings" Codebehind="PspCodeSettings.aspx.cs" %>

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
            pspCodeGridview.StartEditRow(pspCodeGridview.GetFocusedRowIndex());
        });

        shortcut.add("CTRL+N", function () {
            pspCodeGridview.AddNewRow();
        });

        shortcut.add("del", function () {

            if (confirm("Wollen Sie diesen PSPCode wirklich löschen?")) {
                if (pspCodeGridview.GetFocusedRowIndex() > -1) {
                    pspCodeGridview.GetRowValues(pspCodeGridview.GetFocusedRowIndex(), 'Oid', OnGetRowValues);
                }
            }
        });

        shortcut.add("CTRL+S", function () {
            pspCodeGridview.UpdateEdit();
        });

        shortcut.add("CTRL+C", function () {
            pspCodeGridview.CancelEdit();
        });

        shortcut.add("esc", function () {
            pspCodeGridview.CancelEdit();
        });

        shortcut.add("CTRL+Right", function () {
            pspCodeGridview.NextPage();
        });

        shortcut.add("CTRL+Left", function () {
            pspCodeGridview.PrevPage();
        });


        function CustomButtonClicked() {
            if (confirm("Wollen Sie diesen PSPCode wirklich löschen?")) {
                if (pspCodeGridview.GetFocusedRowIndex() > -1) {
                    pspCodeGridview.GetRowValues(pspCodeGridview.GetFocusedRowIndex(), 'Oid', OnGetRowValues);
                }
            }
        }

        function OnGetRowValues(values) {
            PageMethods.hasPspCodeRecords(values, hasRecords);
        }

        function hasRecords(result)
        {
            if (result == true) {
                alert("Dieser PSP-Code ist bereits mit Records verknüpft und kann daher nicht gelöscht werden!");
            }
            else {
                pspCodeGridview.DeleteRow(pspCodeGridview.GetFocusedRowIndex());
            }
        }

    </script>

    <fieldset>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True" />
        <legend>Psp Codes</legend>
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

        <dx:ASPxGridView ID="PspCodeGridview" ClientInstanceName="pspCodeGridview" ClientIDMode="Static" runat="server" DataSourceID="PspCodesXpoData" EnableViewState="false"
            align="center" KeyFieldName="Oid" KeyboardSupport="true" Width="100%" AutoGenerateColumns="false" OnStartRowEditing="grid_StartRowEditing"  OnRowValidating="grid_RowValidating">
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
                <dx:GridViewCommandColumn VisibleIndex="0" Width="50px" Caption=" " ShowEditButton="true" ShowClearFilterButton="true" ShowNewButtonInHeader="True" ButtonType="Image">
                    <CustomButtons>
                        <dx:GridViewCommandColumnCustomButton Text=" " ID="del" Image-Url="~/Icons/delete.png" Image-AlternateText="delete" Image-ToolTip="delete">
                         </dx:GridViewCommandColumnCustomButton>
                    </CustomButtons>
                </dx:GridViewCommandColumn>
                <dx:GridViewDataTextColumn FieldName="Oid" Caption="UserId" Visible="false">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn VisibleIndex="1" FieldName="PspCodeNumber" Caption="PSP Code"
                    EditFormSettings-VisibleIndex="0" Width="100px">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn VisibleIndex="2" FieldName="PspCodeName" Caption="PSP Bezeichnung"
                    EditFormSettings-VisibleIndex="1">
                    <DataItemTemplate>
                        <div style="width: 400px; overflow: hidden; white-space: nowrap;" title="<%# Eval("PspCodeName") %>">
                            <%# Eval("PspCodeName")%>
                        </div>
                    </DataItemTemplate>
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataComboBoxColumn VisibleIndex="3" FieldName="Projects!Key" Caption="zugeordnetes Projekt"
                    EditFormSettings-VisibleIndex="2" Width="100px">
                    <PropertiesComboBox DataSourceID="ProjectsXpoData" TextField="ProjectName" ValueField="Oid"
                        IncrementalFilteringMode="StartsWith" />
                </dx:GridViewDataComboBoxColumn>
                <dx:GridViewDataComboBoxColumn VisibleIndex="4" FieldName="Status!Key" Caption="Status"
                    EditFormSettings-VisibleIndex="3" Width="100">
                    <PropertiesComboBox DataSourceID="StatusXpoData" TextField="StatusName" ValueField="Oid" IncrementalFilteringMode="StartsWith" />
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
            <Styles AlternatingRow-Enabled="True" FocusedRow-BackColor="#0077C0" />
            <SettingsEditing PopupEditFormWidth="600" PopupEditFormModal="true" NewItemRowPosition="Bottom" />
            <SettingsText GroupPanel="Um zu gruppieren, ziehen Sie bitte die entsprechende Spalte hier her"
                CommandUpdate="Save" CommandClearFilter="clear Filter"/>
            <Settings ShowFilterRow="True" EnableFilterControlPopupMenuScrolling="true" ShowGroupPanel="True"
                ShowFooter="True" ShowGroupFooter="VisibleIfExpanded"/> 
             <SettingsPager PageSize="20" Visible="true" />
            <ClientSideEvents CustomButtonClick="CustomButtonClicked"/>
        </dx:ASPxGridView>
    </fieldset>
        <dx:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="PspCodeGridview"
        PaperKind="A4" MaxColumnWidth="200">
        <Styles>
            <Cell Font-Size="Small">
            </Cell>
        </Styles>
    </dx:ASPxGridViewExporter>
    <dx:XpoDataSource ID="PspCodesXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.PspCodes">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="ProjectsXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Projects">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="StatusXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Status">
    </dx:XpoDataSource>
</asp:Content>
