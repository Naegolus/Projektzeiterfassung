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

<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="false"
    CodeBehind="ProjecttypeSettings.aspx.cs" Inherits="com.commend.tools.PZE.ProjecttypeSettings" %>

<%@ Register Assembly="DevExpress.Web.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dx" %>
<%@ Register Assembly="DevExpress.Xpo.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Xpo" TagPrefix="dx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <fieldset>
        <legend>Projekt-Typen</legend>
        <dx:ASPxGridView ID="ProjectTypeGridview" ClientIDMode="Static" runat="server" EnableRowsCache="false" DataSourceID="ProjectTypeXpoData"
            align="center" KeyFieldName="Oid" Width="100%" AutoGenerateColumns="false">
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
                <dx:GridViewDataColumn FieldName="Name">
                </dx:GridViewDataColumn>
            </Columns>
        </dx:ASPxGridView>
        <dx:XpoDataSource ID="ProjectTypeXpoData" runat="server" TypeName="com.commend.tools.PZE.Data.ProjectType">
        </dx:XpoDataSource>
        
</fieldset>
</asp:Content>