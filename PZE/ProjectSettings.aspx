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

<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="com.commend.tools.PZE._ProjectSettings" CodeBehind="ProjectSettings.aspx.cs" %>

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
            projectsGridview.StartEditRow(projectsGridview.GetFocusedRowIndex());
        });

        shortcut.add("CTRL+N", function () {
            projectsGridview.AddNewRow();
        });

        shortcut.add("delete", function () {
            if (confirm("Wollen Sie dieses Projekt wirklich löschen?")) {
                if (projectsGridview.GetFocusedRowIndex() > -1) {
                    projectsGridview.GetRowValues(projectsGridview.GetFocusedRowIndex(), 'Oid', OnGetRowValues);
                }
            }
        });

        shortcut.add("CTRL+S", function () {
            projectsGridview.UpdateEdit();
        });

        shortcut.add("CTRL+C", function () {
            projectsGridview.CancelEdit();
        });

        shortcut.add("esc", function () {
            projectsGridview.CancelEdit();
        });

        shortcut.add("CTRL+Right", function () {
            projectsGridview.NextPage();
        });

        shortcut.add("CTRL+Left", function () {
            projectsGridview.PrevPage();
        });

        function CustomButtonClicked() {
            if (confirm("Wollen Sie dieses Projekt wirklich löschen?")) {
                if (projectsGridview.GetFocusedRowIndex() > -1) {
                    projectsGridview.GetRowValues(projectsGridview.GetFocusedRowIndex(), 'Oid', OnGetRowValues);
                }
            }
        }
        function hasRecords(result) {
            if (result == true) {
                alert("Dieses Projekt besitzt Records und kann daher nicht gelöscht werden!");
            }
            else {
                projectsGridview.DeleteRow(projectsGridview.GetFocusedRowIndex());
            }
        }

        function OnGetRowValues(values) {
            PageMethods.hasProjectRecords(values, hasRecords);
        }

        function OnListBoxSelectionChanged(sender) {
            var selectedItem = sender.GetSelectedItem();
            if (selectedItem != null) {
                dropDownResProj.SetText(selectedItem.text);
            }
        }
        function SynchronizeListBoxValues(dropDown, args) {
            alert("SynchronizeListBoxValues");
            var texts = dropDown.GetText().split(textSeparator);
            var value = GetValueByText(texts);
            lbResProj.selectedItem = value;
        }

        function GetValueByText(text) {
            var actualValue;
            var item = lbResProj.FindItemByText(text);
            if (item != null)
                actualValue = item.value;
            return actualValue;
        }
    </script>
    <fieldset>
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="True" />
        <legend>Projekte</legend>
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
        <dx:ASPxGridView ID="ProjectsGridview" ClientInstanceName="projectsGridview" ClientIDMode="Static" runat="server" DataSourceID="ProjectsXpoData" EnableRowsCache="false"
            align="center" KeyFieldName="Oid" KeyboardSupport="true" Width="100%" AutoGenerateColumns="false"
            OnDetailRowExpandedChanged="DetailRowExpandedChanged" OnStartRowEditing="grid_StartRowEditing" OnRowValidating="grid_RowValidating">
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
                <dx:GridViewCommandColumn VisibleIndex="0" Width="50px" Caption=" " ShowEditButton="true"
                    ShowClearFilterButton="true" ShowNewButtonInHeader="True" ButtonType="Image">
                    <CustomButtons>
                        <dx:GridViewCommandColumnCustomButton Text=" " ID="del" Image-Url="~/Icons/delete.png" Image-AlternateText="delete" Image-ToolTip="delete">
                        </dx:GridViewCommandColumnCustomButton>
                    </CustomButtons>
                </dx:GridViewCommandColumn>
                <dx:GridViewDataTextColumn FieldName="Oid" Visible="false">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn  FieldName="ProjectNumber" Caption="Projekt Nr" Width="100">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataTextColumn FieldName="ProjectName" Caption="Projekt Name">
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataComboBoxColumn FieldName="ResearchProject!Key" Caption="Projekt Nr" Visible="false">
                    <PropertiesComboBox DataSourceID="ResearchProjectsXpoData" TextField="ResearchProjectName" ValueField="Oid"
                        IncrementalFilteringMode="StartsWith" />
                    <EditFormSettings Visible="False"></EditFormSettings>
                </dx:GridViewDataComboBoxColumn>
                <dx:GridViewDataComboBoxColumn Caption="Forschungsprojekt"
                    Visible="true" EditFormSettings-Visible="True" FieldName="ResearchProject!Key">
                    <PropertiesComboBox DataSourceID="ResearchProjectsXpoData" TextField="ResearchProjectName" ValueField="Oid"
                        IncrementalFilteringMode="StartsWith"/>
                </dx:GridViewDataComboBoxColumn>
                <dx:GridViewDataComboBoxColumn FieldName="ProjectType!Key" Caption="Projektart">
                    <PropertiesComboBox DataSourceID="ProjectTypesXpoData" TextField="Name" ValueField="Oid"
                        IncrementalFilteringMode="StartsWith" />
                </dx:GridViewDataComboBoxColumn>
                                
                <dx:GridViewDataColumn FieldName="StartDate" Caption="Start-Datum">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn FieldName="EndDate" Caption="End-Datum">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn FieldName="ResearchPercentage" Caption="Forschungsanteil">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn FieldName="CostEstimate" Caption="Plankosten">
                </dx:GridViewDataColumn>
                <dx:GridViewDataComboBoxColumn FieldName="Status!Key" Caption="Status" Width="100">
                    <PropertiesComboBox DataSourceID="StatusXpoData" TextField="StatusName" ValueField="Oid" IncrementalFilteringMode="StartsWith" />
                </dx:GridViewDataComboBoxColumn>
                <dx:GridViewDataTextColumn Visible="true" Caption="Tätigkeiten">
                    <EditItemTemplate>
                        <dx:ASPxDropDownEdit runat="server" Width="210px" Visible="true">
                            <DropDownWindowTemplate>
                                <dx:ASPxListBox ID="LBActivities" SelectionMode="CheckColumn" runat="server" Width="100%"
                                    DataSourceID="ActivitiesXpoData" TextField="ActivityName" ValueField="ActivityNumber"
                                    OnDataBound="LBActivities_DataBound" OnSelectedIndexChanged="LBActivities_SelIndexChanged">
                                </dx:ASPxListBox>
                            </DropDownWindowTemplate>
                        </dx:ASPxDropDownEdit>
                    </EditItemTemplate>
                </dx:GridViewDataTextColumn>
                <dx:GridViewDataColumn FieldName="Activatable" Caption="Aktivierbar">
                </dx:GridViewDataColumn>
                <dx:GridViewDataTextColumn FieldName="AdditionalInfo" Caption="Zus. Info" Width="100" Visible="true" 
                    EditFormSettings-ColumnSpan="2"/>                
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
            <SettingsText GroupPanel="Um zu gruppieren, ziehen Sie bitte die entsprechende Spalte hier her"
                CommandUpdate="Save" CommandClearFilter="clear Filter" />
            <Settings ShowFilterRow="True" EnableFilterControlPopupMenuScrolling="true"
                ShowFooter="True" ShowGroupFooter="VisibleIfExpanded" />
            <SettingsPager PageSize="20" Visible="true" />
            <Templates>
                <DetailRow>
                    <div style="padding: 3px 3px 2px 3px">
                        <dx:ASPxGridView ID="detailGrid" runat="server" KeyFieldName="Oid" Width="100%" AutoGenerateColumns="false"
                            OnDataBinding="detailGrid_DataBinding">
                            <Columns>
                                <dx:GridViewDataColumn FieldName="ActivityNumber" Caption="Aktivitätsnummer"
                                    VisibleIndex="0" />
                                <dx:GridViewDataColumn FieldName="ActivityName" Caption="Aktivitätsname" VisibleIndex="1" />
                            </Columns>
                        </dx:ASPxGridView>
                    </div>
                </DetailRow>
            </Templates>
            <SettingsDetail ShowDetailRow="true" />
        </dx:ASPxGridView>
    </fieldset>
    <dx:ASPxGridViewExporter ID="gridExport" runat="server" GridViewID="ProjectsGridview"
        PaperKind="A4" MaxColumnWidth="200">
        <Styles>
            <Cell Font-Size="Small">
            </Cell>
        </Styles>
    </dx:ASPxGridViewExporter>
    <dx:XpoDataSource ID="ProjectsXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Projects">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="ResearchProjectsXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.ResearchProjects">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="StatusXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Status">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="ActivitiesXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.Activities">
    </dx:XpoDataSource>
    <dx:XpoDataSource ID="ProjectTypesXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.ProjectType">
    </dx:XpoDataSource>
</asp:Content>
