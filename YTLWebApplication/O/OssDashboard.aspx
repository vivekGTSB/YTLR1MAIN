<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.OssDashboard" Codebehind="OssDashboard.aspx.vb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title> Oss Dashbaord</title>
    <link rel="stylesheet" type="text/css" media="screen" href="css/dashboard.css" />
   <script src="js/jquery.js" type="text/javascript"></script>
    <script src="js/jquery.poshytip.min.js" type="text/javascript"></script>
   
    <style type="text/css">
     
        .btn-primary {
color: #fff;
background-color: #428bca;
border-color: #357ebd;
}
.btn {
display: inline-block;
padding: 6px 12px;
margin-bottom: 0;
font-size: 14px;
font-weight: 400;
line-height: 1.42857143;
text-align: center;
white-space: nowrap;
vertical-align: middle;
cursor: pointer;
-webkit-user-select: none;
-moz-user-select: none;
-ms-user-select: none;
user-select: none;
background-image: none;
border: 1px solid transparent;
border-radius: 4px;
}
btn-sm, .btn-group-sm>.btn {
padding: 5px 10px;
font-size: 12px;
line-height: 1.5;
border-radius: 3px;
}
        .btn-info {
            color: #fff;
            background-color: #5bc0de;
            border-color: #46b8da;
        }
        .btn-success {
color: #1A1919;
background-color: #FFFFFF;
border-color: #46b8da;
}
        .btn-success:hover  {
color: #1A1919;
background-color: #FFFFFF;
border: 1px solid  #46b8da;
}
        .btn-xs, .btn-group-xs>.btn {
padding: 1px 5px;
font-size: 12px;
line-height: 1.5;
border-radius: 3px;
}
        .btn-warning {
color: #fff;
background-color: #f0ad4e;
border-color: #eea236;
}  #loader
        {
             background-color: #04010D;
  color: #FFF;
  height: 14px;
  width: 100px;
  text-align: center;
  margin-left: 385px;
  margin-right: auto;
  
  visibility:hidden;
  margin-top: 3px;
  margin-bottom: -18px;
  font-weight: bold;
  font-size: 12px;
  position: relative;
  border-color: White;
  border-width: 1px;
  border-style: solid;
   
        }

         
        a
  {
    text-decoration:none !important;
  }
    </style>
   
    <style>
   

          .j2
        {
            font-family: Arial;
            color: white;
            font-weight: bold;
            border: 1px solid black;
            background-color: Yellow;
            font-size: 11px;
            position: relative;
            text-align: center;
            padding: 1px;
            cursor: Pointer;
            nowrap: nowrap;
            white-space: nowrap;
            margin: 1px;
            width: 6px;
           height: 6px;
            float: left;
        }     .j12
        {
           font-family: Arial;
            color: black;
            font-weight: bold;
            border: 1px solid black;
            background-color: Yellow;
            font-size: 11px;
            position: relative;
            text-align: center;
            display: inline-block;
            padding: 1px;
            cursor: Pointer;
            nowrap: nowrap;
            white-space: nowrap;
            margin: 1px;
            width: 6px;
           height: 6px;
           
        }
           
        
        .j4
        {
           font-family: Arial;
            color: white;
            font-weight: bold;
            border: 1px solid black;
            background-color: green;
            font-size: 11px;
            position: relative;
            text-align: center;
            padding: 1px;
            cursor: Pointer;
            nowrap: nowrap;
            white-space: nowrap;
            margin: 1px;
            width: 6px;
            height: 6px;
            float: left;
        }
         .j14
        {
             font-family: Arial;
            color: white;
            font-weight: bold;
            border: 1px solid black;
            background-color: green;
            font-size: 11px;
            position: relative;
            text-align: center;
            display: inline-block;
            padding: 1px;
            cursor: Pointer;
            nowrap: nowrap;
            white-space: nowrap;
            margin: 1px;
            width: 6px;
           height: 6px;
           
        }

        .j1
        {
            font-family: Arial;
            color: White;
            font-weight: bold;
            border: 1px solid black;
            background-color: black;
            font-size: 11px;
            position: relative;
            text-align: center;
            display: inline-block;
            padding: 1px;
            cursor: Pointer;
            nowrap: nowrap;
            white-space: nowrap;
            margin: 1px;
            width: 6px;
            height: 6px;
            float: left;
        } .j11
        {
            font-family: Arial;
            color: White;
            font-weight: bold;
            border: 1px solid black;
            background-color: black;
            font-size: 11px;
            position: relative;
            text-align: center;
            display: inline-block;
            padding: 1px;
            cursor: Pointer;
            nowrap: nowrap;
            white-space: nowrap;
            margin: 1px;
         
           
        } 

        .j5
        {
            font-family: Arial;
            color: black;
            font-weight: bold;
            border: 1px solid black;
            background-color: gray;
            font-size: 11px;
            position: relative;
            text-align: center;
            display: inline-block;
            padding: 1px;
            cursor: Pointer;
            nowrap: nowrap;
            white-space: nowrap;
            margin: 1px;
            width: 6px;
           height: 6px;
            float: left;
        }
          .j15
        {
            font-family: Arial;
            color: black;
            font-weight: bold;
            border: 1px solid black;
            background-color: gray;
            font-size: 11px;
            position: relative;
            text-align: center;
            display: inline-block;
            padding: 1px;
            cursor: Pointer;
            nowrap: nowrap;
            white-space: nowrap;
            margin: 1px;
            width: 6px;
           height: 6px;
            
        }


 .j7
        {
            font-family: Arial;
            color: purple;
            font-weight: bold;
            border: 1px solid purple;
            background-color: purple;
            font-size: 11px;
            position: relative;
            text-align: center;
            display: inline-block;
            padding: 1px;
            cursor: Pointer;
            nowrap: nowrap;
            white-space: nowrap;
            margin: 1px;
            width: 6px;
            height: 6px;
            float: left;
        }.j17
        {
            font-family: Arial;
            color: purple;
            font-weight: bold;
            border: 1px solid black;
            background-color: purple;
            font-size: 11px;
            position: relative;
            text-align: center;
            display: inline-block;
            padding: 1px;
            cursor: Pointer;
            nowrap: nowrap;
            white-space: nowrap;
            margin: 1px;
            width: 6px;
            height: 6px;
           
        }
        
        
          .j6
        {
            font-family: Arial;
            color: white;
            font-weight: bold;
            border: 1px solid black;
            background-color: blue;
            font-size: 11px;
            position: relative;
            text-align: center;
            padding: 1px;
            cursor: Pointer;
            nowrap: nowrap;
            white-space: nowrap;
            margin: 1px;
            width: 6px;
           height: 6px;
            float: left;
        }    .j16
        {
            font-family: Arial;
            color: white;
            font-weight: bold;
            border: 1px solid black;
            background-color: Blue;
            font-size: 11px;
            position: relative;
            text-align: center;
            display: inline-block;
            padding: 1px;
            cursor: Pointer;
            nowrap: nowrap;
            white-space: nowrap;
            margin: 1px;
            width: 6px;
           height: 6px;
           
        }  
        
        
        
        
        
        
        
        
        
        
        
           .j3
        {
            font-family: Arial;
            color: white;
            font-weight: bold;
            border: 1px solid black;
            background-color: red;
            font-size: 11px;
            position: relative;
            text-align: center;
            display: inline-block;
            padding: 1px;
            cursor: Pointer;
            nowrap: nowrap;
            white-space: nowrap;
            margin: 1px;
            width: 6px;
            height: 6px;
            float: left;
        }
         .j13
        {
            font-family: Arial;
            color: white;
            font-weight: bold;
            border: 1px solid black;
            background-color: red;
            font-size: 11px;
            position: relative;
            text-align: center;
            display: inline-block;
            padding: 1px;
            cursor: Pointer;
            nowrap: nowrap;
            white-space: nowrap;
            margin: 1px;
            width: 6px;
            height: 6px;
           
        }
    </style>

    <script type="text/javascript">
        $(document).ready(function () {
            $("#loader").css("visibility", "visible");
            LoadJobs();

        });
        function refreshTable() {

            LoadJobs();
           
        }
        function showdiv() {
            $("#loader").css("visibility", "visible");
            $("#loader").text("Refreshing....");
            //            document.getElementById("loader").innerText = "Refreshing....";
            //            document.getElementById("loader").textContent = "Refreshing....";
           $("#loader").css("visibility", "visible");

        }
        function LoadJobs() {
            $.ajax({
                type: "GET",
                url: "GetOssT1.aspx",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: OnLoadJobs,
                failure: function (response) {
                    alert(response);
                }
            });
        }
        function OnLoadJobs(response) {
            $("#dashboard  tbody  td").html("");
            jQuery.each(response.data, function (i, val) {
                var tdNode = $("#" + val[0]);
                var sts = "--";
                var s1 = "";
                var s2 = "";
                var sa = "";
                switch (val[3]) {
                    case "SA":
                        s1 = "3.0519";
                        s2 = "101.5003";
                        sa = "DEPOT - SHAH ALAM (SA)";
                        break;
                    case "MY10580":
                        s1 = "3.0519";
                        s2 = "101.5003";
                        sa = "MY10580 - SHAH ALAM";
                        break;

                    case "KW":
                        s1 = "4.7721";
                        s2 = "101.1094";
                        sa = "KANTHAN WORKS (KW)";
                        break;
                    case "MY10010":
                        s1 = "4.7721";
                        s2 = "101.1094";
                        sa = "MY10010 - KANTHAN WORKS";
                        break;

                    case "LW":
                        s1 = "6.4199";
                        s2 = "99.7619";
                        sa = "LANGKAWI WORKS (LW)";
                        break;
                    case "MY10020":
                        s1 = "6.4199";
                        s2 = "99.7619";
                        sa = "MY10020 - LANGKAWI WORKS";
                        break;

                    case "RW":
                        s1 = "3.3094";
                        s2 = "101.5715";
                        sa = "RAWANG WORKS (RW)";
                        break;
                    case "MY10000":
                        s1 = "3.3094";
                        s2 = "101.5715";
                        sa = "MY10000 - RAWANG WORKS";
                        break;
                    case "PGT":
                        s1 = "1.4377";
                        s2 = "103.9126";
                        sa = "TERMINAL - PASIR GUDANG (PGT)";
                        break;
                    case "MY10030":
                        s1 = "1.4377";
                        s2 = "103.9126";
                        sa = "MY10030 - TERMINAL PASIR GUDANG";
                        break;

                    case "WPT":
                        s1 = "2.9784";
                        s2 = "101.3299";
                        sa = "TERMINAL - WEST PORT (WPT)";
                        break;
                    case "MY10610":
                        s1 = "2.9784";
                        s2 = "101.3299";
                        sa = "MY10610 - TERMINAL - WEST PORT";
                        break;



                }

                switch (val[12]) {
                    case "0":
                        if (val[5] != "") {
                            sts = "Waiting To Process";
                        } else {
                            sts = "Waiting for Ship To Code";
                        }

                        break;
                    case "1":
                        sts = "No GPS Device";
                        break;
                    case "2":
                        sts = "Pending Destination Set Up";
                        break;
                    case "3":
                        sts = "In Progress";
                        break;
                    case "4":
                        sts = "Geofence In";
                        break;
                    case "5":
                        sts = "Inside Geofence";
                        break;
                    case "6":
                        sts = "Geofence Out";
                        break;
                    case "7":
                        sts = "Delivery Completed";
                        break;
                    case "8":
                        sts = "Delivery Completed (E)";
                        break;

                    case "10":
                        sts = "Timeout";
                        break;
                    case "11":
                        sts = "Reprocess Job";
                        break;
                    case "12":
                        sts = "Delivery Completed (D)";
                        break;
                    case "13":
                        sts = "Delivery Completed (P)";
                        break;
                    case "14":
                        sts = "No GPS Data";
                        break;
                }

                var d1 = val[7].split(",")[0];
                var d2 = val[7].split(",")[1];
                var p;
                var s;
                var d;
                   if (val[13] == 1 || val[6] == "--") {
                    if (val[6] == "--") {
                        title = "DN NO: " + val[8] + "<br/>Patch No: " + val[1] + "<br/>Plate No: " + val[2] + "<br/>Driver: " + val[10] + "<br/>Quantity: " + val[9] + "<br/><br/>Source: <a href='https://maps.google.com/maps?f=q&amp;hl=en&amp;q=" + s1 + "+" + s2 + "&amp;om=1&amp' target='_blank'><b style='color: white;text-decoration: none !important;'>" + sa + "</b></a><br/>Destination:" + val[6] + " (" + val[5] + ")<br/><br/>Weight Out Time: " + val[4] + "<br/>ATA: " + val[11] + "<br/><br/>Status: " + sts;
                    }
                    else {
                        title = "DN NO: " + val[8] + "<br/>Patch No: " + val[1] + "<br/>Plate No: " + val[2] + "<br/>Driver: " + val[10] + "<br/>Quantity: " + val[9] + "<br/><br/>Source: <a href='https://maps.google.com/maps?f=q&amp;hl=en&amp;q=" + s1 + "+" + s2 + "&amp;om=1&amp' target='_blank'><b style='color: white;text-decoration: none !important;'>" + sa + "</b></a><br/>Destination: <a href='https://maps.google.com/maps?f=q&amp;hl=en&amp;q=" + d1 + "+" + d2 + "&amp;om=1&amp' target='_blank'><b style='color: white;text-decoration: none !important;'>" + val[6] + " (" + val[5] + ")</b></a> <br/><br/>Weight Out Time: " + val[4] + "<br/>ATA: " + val[11] + "<br/><br/>Status: " + sts;
                    }


                }
                else {
                    if ((val[15] != "--" && val[15] != "0") && (val[14] != "--")) {
                        title = "DN NO: " + val[8] + "<br/>Patch No: " + val[1] + "<br/>Plate No: " + val[2] + "<br/>Driver: " + val[10] + "<br/>Quantity: " + val[9] + "<br/><br/>Source: <a href='https://maps.google.com/maps?f=q&amp;hl=en&amp;q=" + s1 + "+" + s2 + "&amp;om=1&amp' target='_blank'><b style='color: white;text-decoration: none !important;'>" + sa + "</b></a><br/>Destination:  <a href='https://maps.google.com/maps?f=q&amp;hl=en&amp;q=" + d1 + "+" + d2 + "&amp;om=1&amp' target='_blank'><b style='color: white;text-decoration: none !important;'>" + val[6] + " (" + val[5] + ")</b></a><br/>Distance:<a href='https://maps.google.com?saddr=" + s1 + "+" + s2 + "&&daddr=" + d1 + "," + d2 + "&&dirflg=d' target='_blank'><b style='color: orange;text-decoration: none !important;padding-left:10px;'>" + val[15] + " KM   (" + val[16] + ")</b></a><br/>ETA: " + val[17] + " <br/><br/>Weight Out Time: " + val[4] + "<br/>ATA: " + val[11] + "<br/>Duration: " + val[14] + "<br/><br/>Status: " + sts;
                    } else {

                        if (val[14] != "--") {

                            title = "DN NO: " + val[8] + "<br/>Patch No: " + val[1] + "<br/>Plate No: " + val[2] + "<br/>Driver: " + val[10] + "<br/>Quantity: " + val[9] + "<br/><br/>Source: <a href='https://maps.google.com/maps?f=q&amp;hl=en&amp;q=" + s1 + "+" + s2 + "&amp;om=1&amp' target='_blank'><b style='color: white;text-decoration: none !important;'>" + sa + " </b></a><br/>Destination:  <a href='https://maps.google.com/maps?f=q&amp;hl=en&amp;q=" + d1 + "+" + d2 + "&amp;om=1&amp' target='_blank'><b style='color: white;text-decoration: none !important;'>" + val[6] + " (" + val[5] + ")</b></a> <br/>Distance:<a href='https://maps.google.com?saddr=" + s1 + "+" + s2 + "&&daddr=" + d1 + "," + d2 + "&&dirflg=d' target='_blank'><b style='color: orange;text-decoration: none !important;padding-left:10px;'>Route</b></a><br/><br/>Weight Out Time: " + val[4] + "<br/>ATA: " + val[11] + "<br/><br/>Status: " + sts;
                        }
                        else {
                            if ((val[15] != "--" && val[15] != "0")) {
                                title = "DN NO: " + val[8] + "<br/>Patch No: " + val[1] + "<br/>Plate No: " + val[2] + "<br/>Driver: " + val[10] + "<br/>Quantity: " + val[9] + "<br/><br/>Source: <a href='https://maps.google.com/maps?f=q&amp;hl=en&amp;q=" + s1 + "+" + s2 + "&amp;om=1&amp' target='_blank'><b style='color: white;text-decoration: none !important;'>" + sa + "</b></a><br/>Destination:  <a href='https://maps.google.com/maps?f=q&amp;hl=en&amp;q=" + d1 + "+" + d2 + "&amp;om=1&amp' target='_blank'><b style='color: white;text-decoration: none !important;'>" + val[6] + " (" + val[5] + ")</b></a> <br/>Distance:<a href='https://maps.google.com?saddr=" + s1 + "+" + s2 + "&&daddr=" + d1 + "," + d2 + "&&dirflg=d' target='_blank'><b style='color: orange;text-decoration: none !important;padding-left:10px;'>" + val[15] + " KM   (" + val[16] + ")</b></a><br/>ETA: " + val[17] + " <br/><br/>Weight Out Time: " + val[4] + "<br/>ATA: " + val[11] + "<br/><br/>Status: " + sts;
                            }
                            else {
                                title = "DN NO: " + val[8] + "<br/>Patch No: " + val[1] + "<br/>Plate No: " + val[2] + "<br/>Driver: " + val[10] + "<br/>Quantity: " + val[9] + "<br/><br/>Source: <a href='https://maps.google.com/maps?f=q&amp;hl=en&amp;q=" + s1 + "+" + s2 + "&amp;om=1&amp' target='_blank'><b style='color: white;text-decoration: none !important;'>" + sa + "</b></a><br/>Destination:  <a href='https://maps.google.com/maps?f=q&amp;hl=en&amp;q=" + d1 + "+" + d2 + "&amp;om=1&amp' target='_blank'><b style='color: white;text-decoration: none !important;'>" + val[6] + " (" + val[5] + ")</b></a> <br/>Distance:<a href='https://maps.google.com?saddr=" + s1 + "+" + s2 + "&&daddr=" + d1 + "," + d2 + "&&dirflg=d' target='_blank'><b style='color: orange;text-decoration: none !important;padding-left:10px;'>Route</b></a><br/><br/>Weight Out Time: " + val[4] + "<br/>ATA: " + val[11] + "<br/><br/>Status: " + sts;
                            }

                           
                        }


                    }

                }


                span = $('<span id=' + val[0] + ' title="' + title + '" class="j' + val[13] + '"></span>');

                tdNode.append(span);
            });
           $("#loader").css("visibility", "hidden");
            $('span').poshytip({
                className: 'tip-twitter',
                showTimeout: 1,
                alignTo: 'target',
                alignX: 'center',
                offsetY: 5,
                allowTipHover: true,
                fade: false,
                slide: false
            });

        }
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div>
          
          
            <table id="dashboard">
                <%=htmlsb.ToString() %>
            </table>
         
        </div>
       
      
    
    </form>
</body>
</html>
