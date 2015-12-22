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

<%@ Page Title="Log In" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" Inherits="com.commend.tools.PZE.Account.Account_Login" Codebehind="Login.aspx.cs" %>
<%@ Register Assembly="DevExpress.Web.v15.1, Version=15.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a"
    Namespace="DevExpress.Web" TagPrefix="dx" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:Panel ID="Panel1" runat="server" DefaultButton="LoginButton">
        <div class="login">
            <fieldset class="login" id="fieldLogin" runat="server">
                <legend>Login</legend>
                <p>
                    <asp:Label ID="UserNameLabel" runat="server" AssociatedControlID="txtUserName">Benutzername:</asp:Label>
                    <asp:TextBox ID="txtUserName" runat="server" CssClass="textEntry" Width="250px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="txtUserName"
                        CssClass="failureNotification" ErrorMessage="User Name is required." ToolTip="User Name is required."
                        ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                </p>
                <p>
                    <asp:Label ID="PasswordLabel" runat="server" AssociatedControlID="txtPassword">Passwort:</asp:Label>
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="passwordEntry" TextMode="Password"
                        Width="250px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="txtPassword"
                        CssClass="failureNotification" ErrorMessage="Password is required." ToolTip="Password is required."
                        ValidationGroup="LoginUserValidationGroup">*</asp:RequiredFieldValidator>
                </p>
                <table width="100%">
                    <tr>
                        <td>
                            <asp:CheckBox ID="chkRememberMe" runat="server" />
                            <asp:Label ID="RememberMeLabel" runat="server" AssociatedControlID="chkRememberMe"
                                CssClass="inline">Angemeldet bleiben</asp:Label>
                        </td>
                        <td align="right" class="submitLinkButton">
                            <dx:ASPxButton ID="LoginButton" runat="server" Text="anmelden" OnClick="LoginButton_Click" EnableDefaultAppearance="false"
                                CssClass="submitLinkButton" ValidationGroup="LoginUserValidationGroup" Font-Size="Medium"
                                Cursor="pointer" />
                        </td>
                    </tr>
                </table>
            </fieldset>
            <div align="center">
                <asp:Literal ID="litInfo" runat="server"></asp:Literal>
            </div>
        </div>
    </asp:Panel>
</asp:Content>
