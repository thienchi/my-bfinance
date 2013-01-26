<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uccommentf.ascx.cs" Inherits="dangdongcmm.uccommentf" %>
<%@ REGISTER TagPrefix="UC" TagName="Pager" Src="ucpager.ascx" %>

<ASP:PANEL ID="pnlList" runat="server" Visible="false">
    <div class="boxright2">
    <div class="title3bound"><img src="../images/ttit-comment.png" alt="Comments" /></div>    
    
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE> 
            <div class="commentf">
                <div class="introduce"><%#Eval("Description") %></div>
                <div class="note"><i><%#Eval("eTimeupdate") %></i>, từ <b><u><%#Eval("Sender_Name") %></u></b></div>
            </div>
        
        </ITEMTEMPLATE>
    </ASP:REPEATER>
    
    <UC:PAGER ID="pagBuilder" runat="server" CssClass="pagBuilder" Infotemplate="" PageSize="10" />
    </div>
</ASP:PANEL>
