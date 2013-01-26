<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucnewst.ascx.cs" Inherits="dangdongcmm.ucnewst" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="ucpager.ascx" %>

<ASP:PANEL ID="pnlList" runat="server" CssClass="relatedl width100">
    <ASP:DATALIST ID="dtlList" runat="server" CssClass="ldatalist" SelectedIndex="0" RepeatLayout="Flow">
        <SELECTEDITEMTEMPLATE>
            <div class="newsselhome" style="margin-bottom:20px;">
               <%#Eval("eFilepreview").ToString()==""?"":("<div class='blockimg'>" + Eval("eFilepreview") + "</div>")%>
               <div class="lname"><a href="<%#Eval("eUrl") %>"><%#Eval("Name")%></a> <%#Eval("eIconex") %></div>
               <div class="intr"><%#Eval("Introduce") %></div>
            </div>
        </SELECTEDITEMTEMPLATE>
        <ITEMTEMPLATE>
            <div class="newsselhome">
                <div class="inlist"><a href="<%#Eval("eUrl") %>"><%#Eval("Name")%></a> <%#Eval("eIconex") %></div>
            </div>
        </ITEMTEMPLATE>
    </ASP:DATALIST>
    <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="" PageSize="5" Visible="false" />
</ASP:PANEL>