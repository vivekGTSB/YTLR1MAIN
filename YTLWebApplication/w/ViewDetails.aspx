<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.ViewDetails" Codebehind="ViewDetails.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>View Details</title>
    <style type="text/css" >
    
    .item {
	-webkit-border-radius: 5px;
	border-radius: 5px;
	display: block;			
	border: 1px solid #AAA;
	padding: 8px;	
	padding-left: 5px;	
	padding-right: 10px;	
	float: left;
	color: #777777;
	text-shadow: 1px 1px 2px rgba(255, 255, 255, 0.65);		
	margin-right: 5px;	
	max-width: .6+ */
	background:px;
	margin-bottom: 10px;
	font-size: 11px;
}

	.item.normal  {
	background:-moz-linear-gradient(top, #FFFFFF 0%, #7589EE 100%); /* FF3.6+ */
	background:-webkit-gradient(linear, left top, left bottom, color-stop(0%,#FFFFFF), color-stop(100%,#7589EE)); /* Chrome,Safari4+ */
	background:-webkit-linear-gradient(top, #FFFFFF 0%,#7589EE 100%); /* Chrome10+,Safari5.1+ */
	background:-o-linear-gradient(top, #FFFFFF 0%,#7589EE 100%); /* Opera11.10+ */
	background:-ms-linear-gradient(top, #FFFFFF 0%,#7589EE 100%); /* IE10+ */
	background:linear-gradient(top, #FFFFFF 0%,#7589EE 100%); /* W3C */
	
	border: 1px solid #A0A0A0;	
	vertical-align: baseline;			

}
.item.active {
	background:-moz-linear-gradient(top, #CBE0F3 0%, #4994D7 2%, #066ECD 100%); /* FF3.6+ */
	background:-webkit-gradient(linear, left top, left bottom, color-stop(0%, #CBE0F3), color-stop(2%,#4994D7), color-stop(100%,#066ECD)); /* Chrome,Safari4+ */
	background:-webkit-linear-gradient(top, #CBE0F3 0%, #4994D7 2%, #066ECD 100%); /* Chrome10+,Safari5.1+ */
	background:-o-linear-gradient(top, #CBE0F3 0%, #4994D7 2%, #066ECD 100%); /* Opera11.10+ */
	background:-ms-linear-gradient(top, #CBE0F3 0%, #4994D7 2%, #066ECD 100%); /* IE10+ */
	background:linear-gradient(top, #CBE0F3 0%, #4994D7 2%, #066ECD 100%); /* W3C */
	filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#4994D7', endColorstr='#066ECD');
	-ms-filter: "progid:DXImageTransform.Microsoft.gradient(startColorstr='#4994D7', endColorstr='#066ECD')";
	color: #FFF;
	border: 1px solid #03437C;		
	text-shadow: 1px 1px 2px rgba(0, 0, 0, 0.5);			
}	
	
.item.active .value {
    font-size: 20px;	
	display: block;		
	margin-top: 0.2em;
}
	
.item.normal .value {
	color: #444444;
    font-size: 18px;			
	display: block;
	margin-top: 0.2em;
}

.item.normal .value.small {
    font-size: 10px;			
	margin-top: 0.1em;
}

.info_box {
	-webkit-border-radius: 5px;
	border-radius: 5px;			
	background: #fff;
	border: 1px solid #AAA;
	padding: 4px;
	margin-top: 0.1em;
	
}

.dropdown{
	font-size: 12px;
	display: block;
	padding: 3px;
	border: solid 1px #AAA;	
	width: 400px;			
}
.gbutton {
    padding: 0.4em 1em 0.4em 20px;
    position: relative;
    text-decoration: none;
    width:180px;
}	
.gbutton span.ui-icon {
			    left: 0.2em;
			    margin: -8px 5px 0 0;
			    position: absolute;
			    top: 50%;
			}
 .textbox1
        {
        height :20px;
        width: 180px;
        border-right: #cbd6e4 1px solid;
        border-top: #cbd6e4 1px solid;
        border-left: #cbd6e4 1px solid;
        color: #0b3d62;
        border-bottom: #cbd6e4 1px solid;
        }
        
.hor-minimalist-b
{
	font-family: "Verdana";
	font-size: 11px;
	background: #fff;
	margin: 5px;
	width: 99%;
	border-collapse: collapse;
	text-align: left;
}
.hor-minimalist-b th
{
	font-size: 14px;
	font-weight: normal;
	color: #039;
	padding: 5px 3px;
	border-bottom: 2px solid #6678b1;
}
.hor-minimalist-b td
{
	border-bottom: 1px solid #ccc;
	color: #669;
	padding: 6px 8px;
}
.hor-minimalist-b tbody tr:hover td
{
	color: #009;
}
</style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
       <table   id="hor-minimalist-b" class="hor-minimalist-b">
       <%=sb.ToString() %>                  
       </table>
    </div>
    </form>
</body>
</html>
