<%@ Page Language="C#" MasterPageFile="MasterDefault.Master" AutoEventWireup="true" CodeBehind="scontentview.aspx.cs" Inherits="dangdongcmm.scontentview" %>

<ASP:CONTENT ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
            
    <ASP:PANEL ID="pnlInfo" runat="server">
        <ASP:REPEATER ID="rptInfo" runat="server">
            <ITEMTEMPLATE>
                <div class="article">
                    <div class="name"><%#Eval("Name")%></div>
                    <div class="description"><%#Eval("Description")%></div>
                </div>
            </ITEMTEMPLATE>
        </ASP:REPEATER>
    </ASP:PANEL>


</ASP:CONTENT>

