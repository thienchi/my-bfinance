<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucaccesscounter.ascx.cs" Inherits="dangdongcmm.ucaccesscounter" %>

<div>
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
            <table cellspacing="0" cellpadding="0" border="0" width="95%">
                <tr>
                    <td>Hôm nay</td>
                    <td align="right" width="80px"><%#Eval("Counter_ThisDate") %></td>
                </tr>
                <tr>
                    <td>Hôm qua</td>
                    <td align="right"><%#Eval("Counter_Yesterday") %></td>
                </tr>
                <tr>
                    <td>Tuần này</td>
                    <td align="right"><%#Eval("Counter_ThisWeek") %></td>
                </tr>
                <tr>
                    <td>Tháng này</td>
                    <td align="right"><%#Eval("Counter_ThisMonth") %></td>
                </tr>
                <tr>
                    <td>Tổng cộng</td>
                    <td align="right"><%#Eval("Counter_Total") %></td>
                </tr>
            </table>
        </ITEMTEMPLATE>
    </ASP:REPEATER>

- Đang truy cập: <B><ASP:LABEL id="currentaccess" runat="server"></ASP:LABEL></B>
<!--- Pageview: <B><ASP:LABEL id="pageview" runat="server"></ASP:LABEL></B>-->
</div>