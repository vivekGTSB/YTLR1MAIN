<%@ Page Language="VB" AutoEventWireup="false" Inherits="YTLWebApplication.kokhaw_fleet_help" Codebehind="help.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        body
        {
            background-color: #f5f5f5;
            font-family: Arial;
        }
        img
        {
            background-color: White;
        }
        .img
        {
            width: 900px;
        }
        a
        {
            color: #0088cc;
            cursor: pointer;
        }
        a:hover, a:focus
        {
            color: #005580;
        }
        .Loading
        {
            display: none;
            width: 900px;
        }
        .bs-docs-sidenav
        {
            width: 228px;
            margin: 30px 0 0;
            padding: 0;
            background-color: #fff;
            -webkit-border-radius: 6px;
            -moz-border-radius: 6px;
            border-radius: 6px;
            -webkit-box-shadow: 0 1px 4px rgba(0,0,0,.065);
            -moz-box-shadow: 0 1px 4px rgba(0,0,0,.065);
            box-shadow: 0 1px 4px rgba(0,0,0,.065);
        }
        .bs-docs-sidenav > li > a
        {
            display: block;
            width: 190px \9;
            margin: 0 0 -1px;
            padding: 8px 14px;
            border: 1px solid #e5e5e5;
        }
        .bs-docs-sidenav > li:first-child > a
        {
            -webkit-border-radius: 6px 6px 0 0;
            -moz-border-radius: 6px 6px 0 0;
            border-radius: 6px 6px 0 0;
        }
        .bs-docs-sidenav > li:last-child > a
        {
            -webkit-border-radius: 0 0 6px 6px;
            -moz-border-radius: 0 0 6px 6px;
            border-radius: 0 0 6px 6px;
        }
        .bs-docs-sidenav > .active > a
        {
            position: relative;
            z-index: 2;
            padding: 9px 15px;
            border: 0;
            text-shadow: 0 1px 0 rgba(0,0,0,.15);
            -webkit-box-shadow: inset 1px 0 0 rgba(0,0,0,.1), inset -1px 0 0 rgba(0,0,0,.1);
            -moz-box-shadow: inset 1px 0 0 rgba(0,0,0,.1), inset -1px 0 0 rgba(0,0,0,.1);
            box-shadow: inset 1px 0 0 rgba(0,0,0,.1), inset -1px 0 0 rgba(0,0,0,.1);
        }
        /* Chevrons */
        .bs-docs-sidenav .icon-chevron-right
        {
            float: right;
            margin-top: 2px;
            margin-right: -6px;
            opacity: .25;
        }
        .bs-docs-sidenav > li > a:hover
        {
            background-color: #f5f5f5;
        }
        .bs-docs-sidenav a:hover .icon-chevron-right
        {
            opacity: .5;
        }
        .bs-docs-sidenav.affix
        {
            top: 40px;
        }
        .bs-docs-sidenav.affix-bottom
        {
            position: absolute;
            top: auto;
            bottom: 270px;
        }
        .nav-list > .active > a, .nav-list > .active > a:hover, .nav-list > .active > a:focus
        {
            color: #ffffff;
            text-shadow: 0 -1px 0 rgba(0, 0, 0, 0.2);
            background-color: #0088cc;
        }
    </style>
    <script type="text/javascript" language="javascript" src="js/jquery.js"></script>
    <script type="text/javascript">

        $(function () {
            $('.btn').click(function () {
                var URL = $(this).attr('data-values');
                var CurrentImg = $('#getImg').attr('src');


                $(".nav-list li").removeClass("active");
                $(this).closest("li").addClass("active");

                if (URL != CurrentImg) {
                    $('#DivLoading').show();
                    $('#getImg').hide();
                }

                $('#getImg').attr('src', URL);
                $('#getImg').load(function () {
                    $('#DivLoading').hide();
                    $('#getImg').show();
                });

            });
        });

    </script>
</head>
<body>
    <form id="form1" runat="server">
    <center>
        <div>
            
            <div style="float: left; margin-left:40px">
                <h3>
                    User Handouts</h3>
                <div class="span3 bs-docs-sidebar">
                    <ul class="nav nav-list bs-docs-sidenav affix-top" style="list-style: none; text-align: left; width:190px;font-size: 14px;">
                        <li class = "active"><a class="btn" data-values="images/Fleet BETA@ maintenance-01.png">Servicing Management</a></li>
                        <li><a class="btn" data-values="images/Fleet BETA@ maintenance-02.png">Vehicle Documents
                            Date Management</a></li>
                        <li><a class="btn" data-values="images/Fleet BETA@ maintenance-03.png">Trailer Management</a></li>
                        <li id = "tol1" runat = "server" visible = "false"><a class="btn" data-values="images/Fleet Beta @ Toll-01.png">Toll Management</a></li>
                        <li id = "tol2" runat = "server" visible = "false"><a class="btn" data-values="images/Fleet Beta @ Toll-02.png">Toll Fare Management</a></li> 
                    </ul>
                </div>
            </div>
            <div style="float: left; margin-left: 80px">
                <div id="DivLoading" class="Loading">
                    <div style="margin-top: 50px">
                        <b id="bloading">Loading... </b>
                    </div>
                </div>
                <div>
                    <img id="getImg" alt="" class="img" src="images/Fleet BETA@ maintenance-01.png" /></div>
            </div>
        </div>
    </center>
    </form>
</body>
</html>
