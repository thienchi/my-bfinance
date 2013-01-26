<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ucfilebanner.ascx.cs" Inherits="dangdongcmm.cmm.ucfilebanner" %>
<%@ IMPORT Namespace="dangdongcmm.utilities" %>

<div class="rowhor">
    <div class="title">
        <div>&nbsp;</div>
        <div class="FBfileattach">
            <ASP:RADIOBUTTONLIST ID="FB_radSelectresource" runat="server" RepeatDirection="Horizontal" CssClass="ctrlrad" onchange="FBSelectsource()">
                <ASP:LISTITEM Value="0" Selected="True">Tải lên</ASP:LISTITEM>
                <ASP:LISTITEM Value="1">Đường dẫn</ASP:LISTITEM>
            </ASP:RADIOBUTTONLIST>
        </div>
    </div>
    <div class="boxctrll">
        <div class="FBfileattach">
            <ASP:HIDDENFIELD ID="FB_txtFileurl" runat="server" />
            <ASP:HIDDENFIELD ID="FB_txtUp" runat="server" />
            <table class="fbhead">
                <tr>
                    <td class="fbbase">Hình ảnh</td>
                    <td class="fbnote">Tiêu đề</td>
                    <td class="fbsort">Thứ tự</td>
                    <td class="fbcomd"></td>
                </tr>
                <tr>
                    <td class="fbbase">
                        <div id="FB_pnlUpload"><input type="file" id="FB_fileUploadbase" name="FB_fileUploadbase" /></div>
                        <div id="FB_pnlGet"><input type="text" id="FB_fileGetbase" value="http://" /></div>
                    </td>
                    <td class="fbnote"><input type="text" id="FB_fileUploadsubject" /></td>
                    <td class="fbsort"><input type="text" id="FB_fileUploadsort" value="1" onkeypress="javascript:return UINumber_In(event);" onblur="UINumber_Out(this)" /></td>
                    <td class="fbcomd" align="right"><input type="button" id="FB_cmdUpload" value="Nhận" class="buttoncmd" onclick="FBgetfile()" /></td>
                </tr>
                <tr>
                    <td class="fbbase"></td>
                    <td class="fbnote"><div style="padding:5px 0;">Mô tả</div><textarea rows="3" id="FB_fileUploadnote"></textarea></td>
                    <td class="fbsort"></td>
                    <td class="fbcomd"></td>
                </tr>
                <tr>
                    <td class="fbbase"></td>
                    <td class="fbnote"><div style="padding:5px 0;">Đường dẫn liên kết</div><input type="text" id="FB_fileUploadurl" /></td>
                    <td class="fbsort"></td>
                    <td class="fbcomd"></td>
                </tr>
            </table>
            
            <div id="FB_error"></div>
            <div id="FB_containerImage" runat="server" title="0">
            <table id="FB_listImage"></table>
            </div>
        </div>

        <script type="text/javascript" src="../cssscript/jquery.upload-1.0.2.js"></script>
        <script type="text/javascript">
        var FBsource = 0, FBchanged = false;
        function FBSelectsource() {
            FBsource = $('.FBfileattach').find('input[type=radio][name$="$FB_radSelectresource"]:checked').val();
            if(FBsource==0) { $('#FB_pnlUpload').show(); $('#FB_pnlGet').hide(); }    
            else if(FBsource==1) { $('#FB_pnlUpload').hide(); $('#FB_pnlGet').show(); }    
        }
        function FBgetfile() {
            if(FBsource==0) { FBupload(); } else if(FBsource==1) { FBgeturl(); }
        }
        function FBupload() {
            if(VALIDATA.IsNullOrEmpty( $('#FB_fileUploadbase').val()) ) return;
            $('#FB_fileUploadbase').upload('../FPUploadHandler.ashx?up=' + $('#<%=FB_txtUp.ClientID %>').val(), function(res) {
                var json = eval("(" + res.substring(5, res.length - 6) + ")");
                FBaddtolist(json["name"]);
                $('#FB_error').html(json["error"]);
            }, 'html');
        }        
        function FBgeturl() {
            if(VALIDATA.IsNullOrEmpty( $('#FB_fileGetbase').val()) ) return;
            var fileurlbase = $('#FB_fileGetbase').val();
            FBaddtolist(fileurlbase);
        }
        
        var FBindex = Math.abs($('#<%=FB_containerImage.ClientID %>').attr('title'));
        $('#FB_fileUploadsort').val(FBindex);
        function FBaddtolist(fileurlbase) {
            var fileurlabs = fileurlbase;
            fileurlbase = fileurlbase.indexOf('http://')==0 ? fileurlbase : ('<%=CConstants.WEBSITE %>/' + fileurlbase);
            var row = $('<tr id="fbrowi'+FBindex+'" class="fbrowi"></tr>').appendTo('#FB_listImage');
            $('<td title="' + fileurlabs + '" class="fbbase"><img id="fbimag'+FBindex+'" src="' + fileurlbase + '" /></td>').appendTo(row);
            $('<td title="'+$('#FB_fileUploadsubject').val()+']['+$('#FB_fileUploadnote').val()+']['+$('#FB_fileUploadurl').val()+'" class="fbnote"><b>'+$('#FB_fileUploadsubject').val()+'</b><br><i>'+$('#FB_fileUploadnote').val()+'</i><br><a target="_blank" href="'+$('#FB_fileUploadurl').val()+'">'+$('#FB_fileUploadurl').val()+'</a></td>').appendTo(row);
            $('<td title="'+$('#FB_fileUploadsort').val()+'" class="fbsort"><input type="text" value="'+$('#FB_fileUploadsort').val()+'" onkeypress="javascript:return UINumber_In(event);" onblur="UINumber_Out(this);FBresetval(this)" /></td>').appendTo(row);
            $('<td class="fbcomd"><a href="javascript:FBremove(0,'+FBindex+');">x</a></td>').appendTo(row);
            $('#FB_fileUploadbase').val(''); $('#FB_fileGetbase').val(''); $('#FB_fileUploadsubject').val(''); $('#FB_fileUploadnote').val(''); $('#FB_fileUploadurl').val(''); $('#FB_fileUploadsort').val(FBindex++);
            $('#FB_error').html('');
            FBchanged = true;
        }
        function FBremove(e, index) {
            var src = $('#FB_listImage').find('img#fbimag' + index).attr('src').replace('<%=CConstants.WEBSITE %>', '');
            var pid = $('#ctl00_ContentPlaceHolder1_txtId').val();
            $('#FB_cmdUpload').upload('../FPRemoveHandler.ashx?fileurl=' + src + '&pid=' + pid + '&iid=' + e, function(res) {
                $('#FB_listImage').find('#fbrowi' + index).remove();
                $('#FB_error').html('File removed.');
            }, 'html');
            FBchanged = true;
        }
        function FBresetval(e) {
            $(e).parent().attr('title', $(e).val());
        }
        function FBgetfileuploaded() {
            if(!FBchanged) return;
            var json = '', count=0;
            $('#FB_listImage').find('tr').each(function() {
                $(this).find('td').each(function() {
                    json += (count==0? '' : '[') + $(this).attr('title') + ']';
                    count++;
                });
                json += '#';
            });
            json += '[';
            $('#<%=FB_txtFileurl.ClientID %>').val(json);
        }

        </script>
    
    </div>
</div>
