<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucdisplaysetting.ascx.cs" Inherits="dangdongcmm.cmm.ucdisplaysetting" %>

    <ASP:PANEL ID="pnlDisplayorder" runat="server" CssClass="rowhor">
        <div class="title">Thứ tự hiển thị:</div>
        <div class="boxctrls"><ASP:TEXTBOX ID="txtOrderd" runat="server" onkeypress="javascript:return UINumber_In(event);" onblur="UINumber_Out(this)"></ASP:TEXTBOX></div>
        <div class="boxctrlc note"> &nbsp; &nbsp; (để trống để tự sắp xếp)</div>
    </ASP:PANEL>
    <ASP:PANEL ID="pnlStatus" runat="server" CssClass="rowhor">
        <div class="title">Trạng thái hiển thị:</div>
        <div class="boxctrll">
            <ASP:RADIOBUTTONLIST ID="radStatus" runat="server" RepeatDirection="Horizontal" CssClass="ctrlrad">
                <ASP:LISTITEM Value="1" Selected="True">Hiển thị</ASP:LISTITEM>
                <ASP:LISTITEM Value="4">Ẩn</ASP:LISTITEM>
            </ASP:RADIOBUTTONLIST>
        </div>
    </ASP:PANEL>
    <ASP:PANEL ID="pnlMarkas" runat="server" CssClass="rowhor">
        <div class="title">Vị trí hiển thị đặc biệt:</div>
        <div class="boxctrll">
            <ASP:RADIOBUTTONLIST ID="radMarkas" runat="server" RepeatDirection="Horizontal" CssClass="ctrlrad">
                <ASP:LISTITEM Value="0">Bình thường</ASP:LISTITEM>
                <ASP:LISTITEM Value="1">Lên đầu trang chủ</ASP:LISTITEM>
                <ASP:LISTITEM Value="2">Tiêu điểm, nổi bật</ASP:LISTITEM>
            </ASP:RADIOBUTTONLIST>
        </div>
    </ASP:PANEL>
    <ASP:PANEL ID="pnlIcon" runat="server" CssClass="rowhor">
        <div class="title">Biểu tượng kèm theo thông tin:</div>
        <div class="boxctrll"><ASP:RADIOBUTTONLIST ID="radIcon" runat="server" RepeatDirection="Horizontal" RepeatColumns="5" CssClass="ctrlrad"></ASP:RADIOBUTTONLIST></div>
    </ASP:PANEL>
