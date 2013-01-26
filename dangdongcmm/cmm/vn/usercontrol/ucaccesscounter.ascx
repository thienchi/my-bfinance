<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucaccesscounter.ascx.cs" Inherits="dangdongcmm.cmm.ucaccesscounter" EnableViewState="false" %>

<div>
    <div><b>Thống kê tình hình truy cập</b></div>
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
            <table cellspacing="10" cellpadding="0" border="0" width="300px">
                <tr>
                    <td><img src="../images/arrowr2.gif" align="absmiddle" /> Hôm nay</td>
                    <td align="right" width="60px"><%#Eval("Counter_ThisDate") %></td>
                </tr>
                <tr>
                    <td><img src="../images/arrowr2.gif" align="absmiddle" /> Hôm qua</td>
                    <td align="right"><%#Eval("Counter_Yesterday") %></td>
                </tr>
                <tr>
                    <td><img src="../images/arrowr2.gif" align="absmiddle" /> Tuần này</td>
                    <td align="right"><%#Eval("Counter_ThisWeek") %></td>
                </tr>
                <tr>
                    <td><img src="../images/arrowr2.gif" align="absmiddle" /> Tháng này</td>
                    <td align="right"><%#Eval("Counter_ThisMonth") %></td>
                </tr>
                <tr>
                    <td><img src="../images/arrowr2.gif" align="absmiddle" /> Tổng số lượt truy cập</td>
                    <td align="right"><%#Eval("Counter_Total") %></td>
                </tr>
            </table>
        </ITEMTEMPLATE>
    </ASP:REPEATER>

    <div>Hiện có <b><ASP:LABEL id="currentaccess" runat="server"></ASP:LABEL></b> khách trực tuyến</div>
</div>