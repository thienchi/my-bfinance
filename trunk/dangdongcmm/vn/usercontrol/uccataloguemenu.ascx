<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uccataloguemenu.ascx.cs" Inherits="dangdongcmm.uccataloguemenu" %>

<ASP:PANEL ID="pnlList" runat="server" CssClass="group">
    <%--<ul>
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
             <li class="break"><%#Container.ItemIndex == 0 ? "" : "|"%></li><li><a href="/<%#Eval("eUrl2").ToString()%>"><%#Eval("Name")%></a> <%#Eval("eIconex") %></li>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
    </ul>--%>
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
             <%#Container.ItemIndex == 0 ? "" : "|"%><a href="/<%#Eval("eUrl2").ToString()%>"><%#Eval("Name")%></a>|<a href="/<%#Eval("eUrl2").ToString()%>"><%#Eval("Name")%></a><%#Eval("eIconex") %>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
    <hr />
</ASP:PANEL> 
