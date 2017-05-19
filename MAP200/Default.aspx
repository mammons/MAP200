<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MAP200.Default" %>

<!DOCTYPE html>

<html lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>MAP-200 Results</title>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="Default.css" rel="stylesheet" />
</head>
<body>


    <form runat="server">
        <div class="container">
            <div class="row">
                <div class="form-group">
                    <div class="values">
                        <div class="value value--insertion-loss control-label col-md-4">
                            <asp:Label Text="Insertion Loss" runat="server" AssociatedControlID="insertionLossTextBox" />
                            <asp:TextBox ID="insertionLossTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>

                        <div class="value value--return-loss control-label col-md-4">
                            <asp:Label Text="Return Loss" runat="server" AssociatedControlID="returnLossTextBox" />
                            <asp:TextBox ID="returnLossTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>

                        <div class="value value--length control-label col-md-4">
                            <asp:Label Text="Length" runat="server" AssociatedControlID="lengthTextBox" />
                            <asp:TextBox ID="lengthTextBox" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                </div>
            </div>
            <div class="row">
                <div class="btn-group btn-center">
                <asp:Button ID="statusBtn" CssClass="btn btn-default" runat="server" Text="PCT Status" OnClick="statusBtn_Click" />
                <asp:Button ID="startPctBtn" CssClass="btn btn-primary" runat="server" Text="Start PCT" />
                <asp:Button ID="stopPctBtn" runat="server" CssClass="btn btn-danger" Text="Stop PCT" />
                <asp:Button ID="runBtn" runat="server" CssClass="btn btn-default" Text="Run Test" />
                    </div>
            </div>
            <div class="row row--textBox control-label">
                <asp:Label Text="Log" runat="server" AssociatedControlID="logTextBox" />
                <asp:TextBox ID="logTextBox" runat="server" CssClass="form-control" Rows="20" TextMode="MultiLine" />
            </div>
        </div>
    </form>
</body>
</html>
