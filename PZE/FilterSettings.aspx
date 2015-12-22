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

<%@ Page Title="Filter" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="com.commend.tools.PZE.FilterSettings" Codebehind="FilterSettings.aspx.cs" %>

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
            filterGridview.StartEditRow(filterGridview.GetFocusedRowIndex());
        });

        shortcut.add("CTRL+N", function () {
            filterGridview.AddNewRow();
        });

        shortcut.add("delete", function () {

            if (confirm("Wollen Sie wirklich dieses ForschungsProjekt löschen?")) {
                filterGridview.DeleteRow(filterGridview.GetFocusedRowIndex());
            }
        });

        shortcut.add("CTRL+S", function () {
            filterGridview.UpdateEdit();
        });

        shortcut.add("CTRL+C", function () {
            filterGridview.CancelEdit();
        });

        shortcut.add("esc", function () {
            filterGridview.CancelEdit();
        });

        shortcut.add("CTRL+Right", function () {
            filterGridview.NextPage();
        });

        shortcut.add("CTRL+Left", function () {
            filterGridview.PrevPage();
        });


        function CustomButtonClicked() {
            if (confirm("Wollen Sie wirklich dieses ForschungsProjekt löschen?")) {
                filterGridview.DeleteRow(filterGridview.GetFocusedRowIndex());
            }
        }

    </script>
    <fieldset>
        <legend>Filter</legend>
        <table>
            <tr>
                <td>
                    <dx:ASPxButton runat="server" Text ="New" OnClick="newFilter_Click">
                    </dx:ASPxButton>
                </td>
            </tr>
        </table>
        <dx:ASPxGridView ID="FilterGridview" ClientInstanceName="filterGridview" ClientIDMode="Static"
            runat="server" align="center" KeyFieldName="Oid"
            KeyboardSupport="true" Width="100%" AutoGenerateColumns="false" OnRowUpdating="FilterGridView_RowUpdating"
            OnRowInserting="FilterGridview_RowInserting"
            OnRowValidating="grid_RowValidating" EnableViewState="false" OnRowDeleting="OnFilterGridviewRowDeleting">
            <SettingsCommandButton>
                 <EditButton Text=" ">
                     <Image Url="~/Icons/edit.png" AlternateText="edit" ToolTip="edit" />
                 </EditButton>
             </SettingsCommandButton>
            <Columns>
                <dx:GridViewCommandColumn VisibleIndex="0" Width="50px" Caption=" " ShowEditButton="true">
                    <CustomButtons>
                        <dx:GridViewCommandColumnCustomButton Text=" " ID="del" Image-Url="~/Icons/delete.png" Image-AlternateText="delete" Image-ToolTip="delete">
                         </dx:GridViewCommandColumnCustomButton>
                    </CustomButtons>
                </dx:GridViewCommandColumn>
                <dx:GridViewDataColumn FieldName="Oid" Caption="FilterId" Visible="false">
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn VisibleIndex="1" FieldName="FilterName" Caption="Filter Name"
                    EditFormSettings-VisibleIndex="0" Width="200">
                </dx:GridViewDataColumn>
               <dx:GridViewDataColumn FieldName="FilterExpression" EditFormSettings-Caption=" " UnboundExpression="FilterExpression"
                    UnboundType="String">
                    <EditItemTemplate>
                        <dx:ASPxRoundPanel ID="RoundPanel" runat="server" HeaderText="Filter" Width="400px">
                            <PanelCollection>
                                <dx:PanelContent ID="PanelContent" runat="server">
                                    <dx:ASPxFilterControl ID="FilterControl" FilterExpression='<%# Eval("FilterExpression") %>'
                                        runat="server" ClientInstanceName="filterControl">
                                        <Columns>
                                            <dx:FilterControlColumn PropertyName="User.SurName" DisplayName="Nachname">
                                            </dx:FilterControlColumn>
                                            <dx:FilterControlColumn PropertyName="User.Division.DivisionName" DisplayName="Bereich">
                                            </dx:FilterControlColumn>
                                            <dx:FilterControlColumn PropertyName="User.PersonalId" DisplayName="Pers. Nr">
                                            </dx:FilterControlColumn>
                                            <dx:FilterControlColumn PropertyName="Date" DisplayName="Datum">
                                            </dx:FilterControlColumn>
                                            <dx:FilterControlColumn PropertyName="Duration" DisplayName="Dauer">
                                            </dx:FilterControlColumn>
                                            <dx:FilterControlColumn PropertyName="Project.ProjectNumber" DisplayName="Proj. Nr">
                                            </dx:FilterControlColumn>
                                            <dx:FilterControlColumn PropertyName="Project.ProjectName" DisplayName="Projekt">
                                            </dx:FilterControlColumn>
                                             <dx:FilterControlColumn PropertyName="PspCode.PspCodeNumber" DisplayName="PSP Code">
                                            </dx:FilterControlColumn>
                                            <dx:FilterControlColumn PropertyName="PspCode.PspCodeName" DisplayName="PSP Bezeichnung">
                                            </dx:FilterControlColumn>
                                            <dx:FilterControlColumn PropertyName="Activity.ActivityNumber" DisplayName="Tät. Nr">
                                            </dx:FilterControlColumn>
                                            <dx:FilterControlColumn PropertyName="Activity.ActivityName" DisplayName="Tätigkeit">                  
                                            </dx:FilterControlColumn>
                                            <dx:FilterControlColumn  PropertyName="Memo" DisplayName="Notiz">                  
                                            </dx:FilterControlColumn>
                                            <dx:FilterControlColumn PropertyName="User.ForName" DisplayName="Vorname">
                                            </dx:FilterControlColumn>
                                            <dx:FilterControlColumn PropertyName="User.UserName" DisplayName="Benutzer">
                                            </dx:FilterControlColumn>
                                            <dx:FilterControlColumn PropertyName="EmployeeLocked" DisplayName="Mit. Freig.">
                                            </dx:FilterControlColumn>
                                            <dx:FilterControlColumn PropertyName="LeaderLocked" DisplayName="LT. Freig.">
                                            </dx:FilterControlColumn>
                                            <dx:FilterControlColumn PropertyName="ResearchProject.Number" DisplayName="Res.Proj-Nr.">
                                            </dx:FilterControlColumn>
                                            <dx:FilterControlColumn PropertyName="ResearchProject.Name" DisplayName="Res.Proj-Name.">
                                            </dx:FilterControlColumn>                                       
                                        </Columns>
                                    </dx:ASPxFilterControl>
                                </dx:PanelContent>
                            </PanelCollection>
                        </dx:ASPxRoundPanel>
                    </EditItemTemplate>
                </dx:GridViewDataColumn>
                <dx:GridViewDataColumn FieldName="Public" Caption="Öffentlich">
                </dx:GridViewDataColumn>
            </Columns>
            <SettingsEditing PopupEditFormWidth="600" PopupEditFormModal="true" NewItemRowPosition="Bottom" />
            <%--<Templates>
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
            </Templates>--%>
            <ClientSideEvents CustomButtonClick="CustomButtonClicked" />
            <Styles AlternatingRow-Enabled="True" FocusedRow-BackColor="#0077C0" />
            <SettingsEditing PopupEditFormWidth="600" PopupEditFormModal="true" NewItemRowPosition="Bottom" />
            <SettingsText CommandUpdate="Save" CommandClearFilter="clear Filter" />
            <Settings ShowFilterRow="True" EnableFilterControlPopupMenuScrolling="true" ShowFooter="True"
                ShowGroupFooter="VisibleIfExpanded" />
            <SettingsPager PageSize="20" Visible="true" />
        </dx:ASPxGridView>
    </fieldset>
</asp:Content>
