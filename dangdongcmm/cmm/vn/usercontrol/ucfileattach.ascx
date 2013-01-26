<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucfileattach.ascx.cs" Inherits="dangdongcmm.cmm.ucfileattach" %>
<%@ IMPORT Namespace="dangdongcmm.utilities" %>

<div class="rowhor">
    <div class="title">
        <div>&nbsp;</div>
        <div class="FAfileattach">
            <ASP:RADIOBUTTONLIST ID="FA_radSelectresource" runat="server" RepeatDirection="Horizontal" CssClass="ctrlrad" onchange="FASelectsource()">
                <ASP:LISTITEM Value="0" Selected="True">Tải lên</ASP:LISTITEM>
                <ASP:LISTITEM Value="1">Đường dẫn</ASP:LISTITEM>
            </ASP:RADIOBUTTONLIST>
        </div>
    </div>
    <div class="boxctrll">
        <div class="FAfileattach">
            <ASP:HIDDENFIELD ID="FA_txtFileurl" runat="server" />
            <ASP:HIDDENFIELD ID="FA_txtUp" runat="server" />
            <table class="fahead">
                <tr>
                    <td class="fabase">Hình ảnh</td>
                    <td class="fanote">Ghi chú</td>
                    <td class="fasort">Thứ tự</td>
                    <td class="facomd"></td>
                </tr>
                <tr>
                    <td class="fabase">
                        <div id="FA_pnlUpload"><input type="file" id="FA_fileUploadbase" name="FA_fileUploadbase" /></div>
                        <div id="FA_pnlGet"><input type="text" id="FA_fileGetbase" value="http://" /></div>
                    </td>
                    <td class="fanote"><input type="text" id="FA_fileUploadnote" /></td>
                    <td class="fasort"><input type="text" id="FA_fileUploadsort" value="1" onkeypress="javascript:return UINumber_In(event);" onblur="UINumber_Out(this)" /></td>
                    <td class="facomd" align="right"><input type="button" id="FA_cmdUpload" value="Nhận" class="buttoncmd" onclick="FAgetfile()" /></td>
                </tr>
            </table>
            
            <div id="FA_error"></div>
            <div id="FA_containerImage" runat="server" title="0">
            <table id="FA_listImage"></table>
            </div>
        </div>

        <script type="text/javascript" src="../cssscript/jquery.upload-1.0.2.js"></script>
        <script type="text/javascript">
        var FAsource = 0, FAchanged = false;
        function FASelectsource() {
            FAsource = $('.FAfileattach').find('input[type=radio][name$="$FA_radSelectresource"]:checked').val();
            if(FAsource==0) { $('#FA_pnlUpload').show(); $('#FA_pnlGet').hide(); }    
            else if(FAsource==1) { $('#FA_pnlUpload').hide(); $('#FA_pnlGet').show(); }    
        }
        function FAgetfile() {
            if(FAsource==0) { FAupload(); } else if(FAsource==1) { FAgeturl(); }
        }
        function FAupload() {
            if(VALIDATA.IsNullOrEmpty( $('#FA_fileUploadbase').val()) ) return;
            $('#FA_fileUploadbase').upload('../FPUploadHandler.ashx?up=' + $('#<%=FA_txtUp.ClientID %>').val(), function(res) {
                var json = eval("(" + res.substring(5, res.length - 6) + ")");
                FAaddtolist(json["name"]);
                $('#FA_error').html(json["error"]);
            }, 'html');
        }        
        function FAgeturl() {
            if(VALIDATA.IsNullOrEmpty( $('#FA_fileGetbase').val()) ) return;
            var fileurlbase = $('#FA_fileGetbase').val();
            FAaddtolist(fileurlbase);
        }
        
        var FAindex = Math.abs($('#<%=FA_containerImage.ClientID %>').attr('title'));
        $('#FA_fileUploadsort').val(FAindex);
        function FAaddtolist(fileurlbase) {
            var fileurlabs = fileurlbase;
            fileurlbase = fileurlbase.indexOf('http://')==0 ? fileurlbase : ('<%=CConstants.WEBSITE %>/' + fileurlbase);
            var row = $('<tr id="farowi'+FAindex+'" class="farowi"></tr>').appendTo('#FA_listImage');
            $('<td title="' + fileurlabs + '" class="fabase"><img id="faimag'+FAindex+'" src="' + fileurlbase + '" /></td>').appendTo(row);
            $('<td title="'+$('#FA_fileUploadnote').val()+'" class="fanote"><input type="text" value="'+$('#FA_fileUploadnote').val()+'" onblur="FAresetval(this)" /></td>').appendTo(row);
            $('<td title="'+$('#FA_fileUploadsort').val()+'" class="fasort"><input type="text" value="'+$('#FA_fileUploadsort').val()+'" onkeypress="javascript:return UINumber_In(event);" onblur="UINumber_Out(this);FAresetval(this)" /></td>').appendTo(row);
            $('<td class="facomd"><a href="javascript:FAremove(0,'+FAindex+');">x</a></td>').appendTo(row);
            $('#FA_fileUploadbase').val(''); $('#FA_fileGetbase').val(''); $('#FA_fileUploadnote').val(''); $('#FA_fileUploadsort').val(FAindex++);
            $('#FA_error').html('');
            FAchanged = true;
        }
        function FAremove(e, index) {
            var src = $('#FA_listImage').find('img#faimag' + index).attr('src').replace('<%=CConstants.WEBSITE %>', '');
            var pid = $('#ctl00_ContentPlaceHolder1_txtId').val();
            $('#FA_cmdUpload').upload('../FPRemoveHandler.ashx?fileurl=' + src + '&pid=' + pid + '&iid=' + e, function(res) {
                $('#FA_listImage').find('#farowi' + index).remove();
                $('#FA_error').html('File removed.');
            }, 'html');
            FAchanged = true;
        }
        function FAresetval(e) {
            $(e).parent().attr('title', $(e).val());
        }
        function FAgetfileuploaded() {
            if(!FAchanged) return;
            var json = '', count=0;
            $('#FA_listImage').find('tr').each(function() {
                $(this).find('td').each(function() {
                    json += (count==0? '' : '[') + $(this).attr('title') + ']';
                    count++;
                });
                json += '#';
            });
            json += '[';
            $('#<%=FA_txtFileurl.ClientID %>').val(json);
        }

        </script>
    
    </div>
</div>
