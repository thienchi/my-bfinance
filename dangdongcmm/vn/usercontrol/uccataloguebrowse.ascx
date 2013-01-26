<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uccataloguebrowse.ascx.cs" Inherits="dangdongcmm.uccataloguebrowse" %>
<%@ IMPORT Namespace="dangdongcmm.utilities" %>

<ASP:PANEL ID="pnlList" runat="server">
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
             | <a class="Subtab" href="<%# CFunctions.Get_Definecatrelate(int.Parse(Eval("Cid").ToString()), Queryparam.Defstring.Page) %>?cid=<%#Eval("Id") %> "><%#Eval("Name")%></a> <%#Eval("eIconex") %>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
</ASP:PANEL>