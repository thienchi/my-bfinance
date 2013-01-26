<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucnewsc.ascx.cs" Inherits="dangdongcmm.ucnewsc" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="ucpager.ascx" %>

<ASP:PANEL ID="pnlList" runat="server" CssClass="relatedl width100">
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
            <div class="newsdich">
                <div class="inlist"><a href="d<%#Eval("Cid")%>d<%#Eval("Id")%>d=<%#Eval("eName")%>?ll=vn"><%#Eval("Name")%><%#Eval("Url").ToString() == "" ? "" : (" (" + Eval("Url") + ")")%></a></div>
            </div>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
    <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="" Visible="false" PageSize="0" />
    
</ASP:PANEL>