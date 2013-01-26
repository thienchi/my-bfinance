<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uccomment.ascx.cs" Inherits="dangdongcmm.uccomment" %>

<div class="commentbox">
    <div class="commentboxin">
    
<ASP:PANEL ID="pnlList" runat="server" Visible="false" CssClass="relatedl width100">
    <div class="title1bound">Thảo luận</div>
    <ASP:REPEATER ID="rptList" runat="server">
        <ITEMTEMPLATE>
            <div class="comment" <%#Container.ItemIndex % 2 == 0 ? "style='background-color:#f7f7f7;'" : ""%>>
                <div class="intr"><%#Eval("Description") %></div>
                <div class="note" align="right"><img src="../images/icon_comment.gif" align="absmiddle" /> <b><%#Eval("Sender_Name") %></b>, gửi lúc: <%#Eval("eTimeupdate") %></div>
            </div>
        </ITEMTEMPLATE>
    </ASP:REPEATER>
    <div class="dbreakh"></div>
</ASP:PANEL>

<ASP:UPDATEPANEL ID="UpdatePanel1" runat="server">
    <CONTENTTEMPLATE>
    <div class="title1bound">Gửi thảo luận</div>
    <div class="FLOUTNOTICE"><ASP:LABEL ID="lblError" runat="server"></ASP:LABEL></div>
    <ASP:PANEL ID="pnlForm" runat="server" CssClass="tformin" Style="width:650px">
        <div class="rowhor">
            <div class="title"><span class="require">*</span> Họ tên</div>
            <div class="boxctrlm"><ASP:TEXTBOX ID="txtSender_Name" runat="server"></ASP:TEXTBOX></div>
        </div>
        <div class="rowhor">
            <div class="title">E-mail</div>
            <div class="boxctrlm"><ASP:TEXTBOX ID="txtSender_Email" runat="server"></ASP:TEXTBOX></div>
        </div>
        <div class="rowhor" style="display:none;">
            <div class="title">Điện thoại</div>
            <div class="boxctrlm"><ASP:TEXTBOX ID="txtSender_Phone" runat="server"></ASP:TEXTBOX></div>
        </div>
        <div class="rowhor" style="display:none;">
            <div class="title">Tiêu đề</div>
            <div class="boxctrlm"><ASP:TEXTBOX ID="txtName" runat="server"></ASP:TEXTBOX></div>
        </div>
        <div class="rowhor">
            <div class="title"><span class="require">*</span> Nội dung</div>
            <div class="boxctrlm"><ASP:TEXTBOX ID="txtDescription" runat="server" TextMode="MultiLine" Rows="8" Style="width:97%"></ASP:TEXTBOX></div>
        </div>
        <div class="rowhor">
            <div class="title">Đánh giá bài viết</div>
            <div class="boxctrll"><ASP:RADIOBUTTONLIST ID="radRating" runat="server" RepeatDirection="Horizontal" CssClass="ctrlrad" Visible="true">
                <ASP:LISTITEM Value="1">Dở tệ</ASP:LISTITEM>
                <ASP:LISTITEM Value="2">Tạm được</ASP:LISTITEM>
                <ASP:LISTITEM Value="3">Hay</ASP:LISTITEM>
                <ASP:LISTITEM Value="4">Quá hay</ASP:LISTITEM>
                <ASP:LISTITEM Value="5" Selected="True">Xuất sắc</ASP:LISTITEM>
            </ASP:RADIOBUTTONLIST></div>
        </div>
        <div class="rowhor">
            <div class="title"></div>
        </div>
        <div class="rowhor">
            <div class="title">Mã bảo vệ</div>
            <div class="boxctrls"><ASP:TEXTBOX ID="txtCaptcha" runat="server" CssClass="captcha"></ASP:TEXTBOX></div>
            <div class="boxctrls" style="margin:1px 0 0 20px; background-color:#aaa; text-align:center; width:55px;"><ASP:IMAGE ID="imgCaptcha" runat="server" ImageUrl="~/commup/captcha/NDE5OA==.png" /></div>
            <div class="boxctrls" style="margin:3px 0 0 5px;"><ASP:IMAGEBUTTON ID="cmdCaptcha" runat="server" ImageUrl="~/images/reload.png" OnClick="cmdCaptcha_Click" /></div>
        </div>
        <ASP:HIDDENFIELD ID="hidCheckemail" runat="server" Value="1" />
        <ASP:HIDDENFIELD ID="hidCheckphone" runat="server" Value="1" />
        
        <div class="rowhor">
            <div class="title">&nbsp;</div>
            <div class="boxctrll"><ASP:BUTTON ID="cmdSubmit" runat="server" Text="Gửi đi" CssClass="buttonwbgs" OnClientClick="javascript:return doComment();" OnClick="cmdSubmit_Click" /></div>
        </div>

        <script type="text/javascript" src="../cssscript/validata.js"></script>
        <script type="text/javascript">
        function CM_Validata() {
            var the, txt = '', msg = '';
            VALIDATA.lblError = '<%=lblError.ClientID %>';
            the = VALIDATA.GetObj('<%=txtSender_Name.ClientID %>');
            if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the))){
                txt = VALIDATA.Gettxt(txt, the);
                msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Require_namefl);
            }
            the = VALIDATA.GetObj('<%=txtSender_Email.ClientID %>');
            if(!VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the)) && !VALIDATA.IsEmail(VALIDATA.GetVal(the))){
                txt = VALIDATA.Gettxt(txt, the);
                msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Invalid_email);
            }
            the = VALIDATA.GetObj('<%=txtDescription.ClientID %>');
            if(VALIDATA.IsNullOrEmpty(VALIDATA.GetVal(the))){
                txt = VALIDATA.Gettxt(txt, the);
                msg = VALIDATA.Getmsg(msg, the, VALIDATA.Error, Definephrase.Require_content);
            }
    	    
            if(VALIDATA.Showerror(txt, msg)) return false;
            return true;
        }
        function doComment(){
            if(!CM_Validata()) return false;
            prompt_processing();
            return true;
        }
        </script>
    </ASP:PANEL>

    <script type="text/javascript">
    function Comment(){
        dangdongcmm.UtiMailService.Comment(Comment_Success, onFailed);
    }
    function Comment_Success(e) { close_processing(); }
    function onFailed() { close_processing(); }
    function showCForm() {}
    </script>

    </CONTENTTEMPLATE>
</ASP:UPDATEPANEL>

    </div>
</div>
