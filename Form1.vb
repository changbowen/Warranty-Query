Imports System.Windows.Forms
Imports System.Net

Public Class WarrantyQuery
    Dim SiteAddress As New Dictionary(Of String, String) From
        {{"Lenovo_CN", "http://think.lenovo.com.cn/service/handlers/WarrantyConfigInfo.ashx?Method=WarrantyConfigSearch&MachineNo=<>"},
         {"Lenovo_US", "http://support.lenovo.com/us/en/warrantylookup"},
         {"Dell_US", "http://xserv.dell.com/services/assetservice.asmx"}}

    Structure WarrantyInfo
        Dim target As SerialInfo
        Dim start_date As String
        Dim end_date As String
        Dim raw_str As String

        ''' <summary>
        ''' 
        ''' </summary>
        ''' <param name="i">SerialInfo of target machine</param>
        ''' <param name="s">Start date</param>
        ''' <param name="e">End date</param>
        ''' <param name="raw">Other info as string</param>
        ''' <remarks></remarks>
        Sub New(i As SerialInfo, s As String, e As String, Optional raw As String = "")
            target = i
            start_date = s
            end_date = e
            raw_str = raw
        End Sub
    End Structure

    Structure SerialInfo
        Dim id As Integer
        Dim serial As String
        Dim model As String

        Sub New(i As Integer, s As String, m As String)
            id = i
            serial = s
            model = m
        End Sub
    End Structure

    Enum Website
        Lenovo_CN
        Lenovo_US
        Dell_US
    End Enum

    Public Event SingleQueryReturned(wi As WarrantyInfo)

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="si">An array of SerialInfo.</param>
    ''' <param name="s">One of the websites in Website enumeration.</param>
    ''' <param name="timeout">This is the initial timeout in seconds when waiting for the site to load first. Not applicable to all websites. Default is 25.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Query(ByVal si() As SerialInfo, ByVal s As Website, Optional ByVal timeout As Integer = 25) As List(Of WarrantyInfo)
        'Dim tmpwin As New Form
        'tmpwin.Size = New Drawing.Size(500, 500)
        'tmpwin.Show()
        'tmpwin.WindowState = FormWindowState.Maximized
        'wb.Parent = Me
        'wb.Dock = DockStyle.Fill

        Dim infolist As New List(Of WarrantyInfo)
        Try
            If si.Length = 0 Then Throw New Exception("SerialInfo array is empty.")
            If s = Website.Lenovo_CN Then
                For Each _si In si
                    Try
                        Dim request = WebRequest.CreateHttp(SiteAddress(s.ToString).Replace("<>", _si.model & _si.serial))
                        Dim respstr As String
                        Using resp = request.GetResponse
                            Using reader As New IO.StreamReader(resp.GetResponseStream)
                                respstr = reader.ReadToEnd
                            End Using
                        End Using
                        Dim respxml = Newtonsoft.Json.JsonConvert.DeserializeXNode(respstr, "ResultForm").Root
                        Dim respxml_status = respxml.Element("status").Value
                        Dim rawstr = Newtonsoft.Json.Linq.JObject.Parse(respstr).ToString

                        If respxml_status = "200" Then 'indicates success
                            Dim startdate = Date.MaxValue, enddate = Date.MinValue
                            If respxml.Elements("ServiceData").Any Then
                                Dim tmpnode = respxml.Element("ServiceData")
                                If tmpnode.Elements("ServiceStartDate").Any Then
                                    Dim tmpdate As Date
                                    If Date.TryParse(tmpnode.Element("ServiceStartDate").Value.Trim, tmpdate) Then
                                        If startdate > tmpdate Then startdate = tmpdate
                                    End If
                                End If
                                If tmpnode.Elements("ServiceEndDate").Any Then
                                    Dim tmpdate As Date
                                    If Date.TryParse(tmpnode.Element("ServiceEndDate").Value.Trim, tmpdate) Then
                                        If enddate < tmpdate Then enddate = tmpdate
                                    End If
                                End If
                            ElseIf respxml.Elements("WarrantyData").Any Then
                                For Each ele In respxml.Element("WarrantyData").Elements
                                    If ele.Name.LocalName.ToLower.Contains("startdate") Then
                                        Dim tmpdate As Date
                                        If Date.TryParse(ele.Value, tmpdate) Then
                                            If startdate > tmpdate Then startdate = tmpdate
                                        End If
                                    ElseIf ele.Name.LocalName.ToLower.Contains("enddate") Then
                                        Dim tmpdate As Date
                                        If Date.TryParse(ele.Value, tmpdate) Then
                                            If enddate < tmpdate Then enddate = tmpdate
                                        End If
                                    End If
                                Next
                            Else
                                Throw New Exception("Error getting warranty information." & vbCrLf & rawstr)
                            End If
                            Dim winfo = New WarrantyInfo(_si, If(startdate = Date.MaxValue, "", startdate.ToString("yyyy-MM-dd")), If(enddate = Date.MinValue, "", enddate.ToString("yyyy-MM-dd")), rawstr)
                            infolist.Add(winfo)
                            RaiseEvent SingleQueryReturned(winfo)
                        ElseIf respxml_status = "102" Then 'indicates multiple categoryid found
                            Throw New Exception("Multiple category ID found. Please use lenovo website for this." & vbCrLf & rawstr)
                        ElseIf respxml_status = "103" Then 'this is a redirect
                            Dim request_sub = WebRequest.CreateHttp(respxml.Element("message").Value)
                            Dim resphtml As New HtmlAgilityPack.HtmlDocument
                            Using resp = request_sub.GetResponse
                                resphtml.Load(resp.GetResponseStream, System.Text.Encoding.UTF8)
                            End Using
                            Dim rawstr_sub = ""
                            For Each n In resphtml.DocumentNode.SelectNodes("//*[@class='ssjg140806_left_top_r_list']")
                                rawstr_sub += n.InnerText.Trim.Replace(vbCrLf, "").Replace(" ", "") & vbCrLf
                            Next
                            Dim startdate = resphtml.GetElementbyId("litCreateDate_LK").InnerText
                            Dim enddate = resphtml.GetElementbyId("lblPartEndDate_LK").InnerText

                            Dim winfo = New WarrantyInfo(_si, startdate, enddate, rawstr_sub)
                            infolist.Add(winfo)
                            RaiseEvent SingleQueryReturned(winfo)
                            Stop
                        ElseIf respxml_status = "101" Then 'indicates not found
                            Throw New Exception("Serial number not found." & vbCrLf & rawstr)
                        Else
                            Throw New Exception("Error getting warranty information." & vbCrLf & rawstr)
                        End If
                    Catch ex As Exception
                        Dim winfo = New WarrantyInfo(_si, "", "", ex.Message)
                        infolist.Add(winfo)
                        RaiseEvent SingleQueryReturned(winfo)
                    End Try
                Next
                Return infolist
            ElseIf s = Website.Lenovo_US Then
                Dim request = WebRequest.CreateHttp(SiteAddress(s.ToString))
                Dim realhost, action As String
                Using response1 = request.GetResponse
                    realhost = response1.ResponseUri.Host
                    Dim rsphtml1 As New HtmlAgilityPack.HtmlDocument
                    rsphtml1.Load(response1.GetResponseStream)
                    Dim serialNumberForm = rsphtml1.GetElementbyId("serialNumberForm")
                    action = serialNumberForm.Attributes("action").Value
                End Using

                For Each _si In si
                    Try
                        Dim post = WebRequest.CreateHttp("http://" & realhost & action)
                        post.ContentType = "application/x-www-form-urlencoded; charset=UTF-8"
                        post.Method = "post"
                        Dim postdata = System.Text.Encoding.UTF8.GetBytes("SerialCode=" & _si.serial & If(_si.model IsNot Nothing AndAlso _si.model.Length > 3, "&MachineType=" & _si.model.Substring(0, 4), ""))
                        Dim rsphtml As New HtmlAgilityPack.HtmlDocument
                        Using datastream = post.GetRequestStream
                            datastream.Write(postdata, 0, postdata.Length)
                            Using resp = post.GetResponse()
                                rsphtml.Load(resp.GetResponseStream)
                            End Using
                        End Using
                        Dim rawstr As String = rsphtml.DocumentNode.InnerHtml
                        Dim rsphtml_status = rsphtml.GetElementbyId("errorMessage")
                        If rsphtml_status IsNot Nothing Then
                            If rsphtml_status.Attributes("value").Value = "" Then 'indicates no error msg
                                Dim rawstra = ""
                                For Each n In rsphtml.DocumentNode.SelectNodes("//*[@class='cell2']")
                                    rawstra += n.InnerText.Trim.Replace(vbCrLf, "").Replace(" ", "") & vbCrLf
                                Next
                                Try
                                    Dim wrntynodes = rsphtml.GetElementbyId("warranty_result_div").SelectNodes("div")
                                    Dim winfo = New WarrantyInfo(_si, wrntynodes(0).InnerText.Split(":"c)(1).Trim, wrntynodes(1).InnerText.Split(":"c)(1).Trim, rawstra.Trim)
                                    infolist.Add(winfo)
                                    RaiseEvent SingleQueryReturned(winfo)
                                Catch
                                    Throw New Exception("Error getting warranty information." & vbCrLf & rawstr)
                                End Try
                            Else
                                Throw New Exception(rsphtml_status.Attributes("value").Value & vbCrLf & rawstr)
                            End If
                        Else
                            Throw New Exception("Error getting warranty information." & vbCrLf & rawstr)
                        End If
                    Catch ex As Exception
                        Dim winfo = New WarrantyInfo(_si, "", "", ex.Message)
                        infolist.Add(winfo)
                        RaiseEvent SingleQueryReturned(winfo)
                    End Try
                Next
                Return infolist
            ElseIf s = Website.Dell_US Then
                For Each _si In si
                    Try
                        Dim post = WebRequest.CreateHttp(SiteAddress(s.ToString))
                        post.Host = "xserv.dell.com"
                        post.ContentType = "text/xml; charset=utf-8"
                        post.Method = "post"
                        post.Headers.Add("SOAPAction", "http://support.dell.com/WebServices/GetAssetInformation")
                        Dim poststr = "<?xml version=""1.0"" encoding=""utf-8""?> " &
                                     "<soap:Envelope xmlns:soap=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema""> " &
                                     "<soap:Body> " &
                                     "<GetAssetInformation xmlns=""http://support.dell.com/WebServices/""> " &
                                     "<guid>11111111-1111-1111-1111-111111111111</guid> " &
                                     "<applicationName>Warranty Information Lookup</applicationName> " &
                                     "<serviceTags>#######</serviceTags> " &
                                     "</GetAssetInformation> " &
                                     "</soap:Body> " &
                                     "</soap:Envelope>"
                        poststr = poststr.Replace("#######", _si.serial)
                        Dim postdata = System.Text.Encoding.UTF8.GetBytes(poststr)
                        Dim rspxml As XDocument
                        Using datastream = post.GetRequestStream
                            datastream.Write(postdata, 0, postdata.Length)
                            Using resp = post.GetResponse()
                                rspxml = XDocument.Load(resp.GetResponseStream)
                            End Using
                        End Using

                        Dim root = rspxml.Descendants(XName.Get("GetAssetInformationResult", "http://support.dell.com/WebServices/"))(0)
                        Dim ns = root.GetDefaultNamespace
                        If root.Elements(ns + "Asset").Any Then
                            Dim startdate = Date.MaxValue, enddate = Date.MinValue
                            For Each entitle In root.Element(ns + "Asset").Descendants(ns + "EntitlementData")
                                Dim tmpdate As Date
                                If Date.TryParse(entitle.Element(ns + "StartDate").Value, tmpdate) Then
                                    If startdate > tmpdate Then startdate = tmpdate
                                End If
                                If Date.TryParse(entitle.Element(ns + "EndDate").Value, tmpdate) Then
                                    If enddate < tmpdate Then enddate = tmpdate
                                End If
                            Next
                            Dim winfo = New WarrantyInfo(_si, If(startdate = Date.MaxValue, "", startdate.ToString("yyyy-MM-dd")), If(enddate = Date.MinValue, "", enddate.ToString("yyyy-MM-dd")), root.ToString)
                            infolist.Add(winfo)
                            RaiseEvent SingleQueryReturned(winfo)
                        Else
                            Throw New Exception("Error getting warranty information." & vbCrLf & rspxml.ToString)
                        End If
                    Catch ex As Exception
                        Dim winfo = New WarrantyInfo(_si, "", "", ex.Message)
                        infolist.Add(winfo)
                        RaiseEvent SingleQueryReturned(winfo)
                    End Try
                Next
                Return infolist
            Else
                Throw New Exception("Site not supported.")
            End If
        Catch ex As Exception
            infolist.Add(New WarrantyInfo(Nothing, Nothing, Nothing, ex.Message))
            Return infolist
        Finally
            Application.DoEvents()
        End Try
    End Function

    ' ''' <summary>
    ' ''' Wait for element to exist / have innertext. Otherwise throws exception.
    ' ''' </summary>
    ' ''' <param name="b"></param>
    ' ''' <param name="ele_id"></param>
    ' ''' <param name="seconds">Put zero here will cause an ArgumentException at runtime.</param>
    ' ''' <param name="accept_null">If the element's innertext might be nothing, specify true to skip checking innertext.</param>
    ' ''' <returns></returns>
    ' ''' <remarks></remarks>
    'Private Function WaitForElement(b As WebBrowser, ele_id As String, seconds As Integer, accept_null As Boolean) As HtmlElement
    '    If seconds = 0 Then
    '        Throw New ArgumentException("Seconds cannot be zero.")
    '    End If
    '    For i = 1 To seconds
    '        If Not IsNothing(b.Document) Then
    '            Dim ele = b.Document.GetElementById(ele_id)
    '            If Not IsNothing(ele) Then
    '                If accept_null Then
    '                    Return ele
    '                Else
    '                    If Not IsNothing(ele.InnerText) Then
    '                        Return ele
    '                    Else
    '                        Threading.Thread.Sleep(1000)
    '                        Application.DoEvents()
    '                    End If
    '                End If
    '            Else
    '                Threading.Thread.Sleep(1000)
    '                Application.DoEvents()
    '            End If
    '        Else
    '            Threading.Thread.Sleep(1000)
    '            Application.DoEvents()
    '        End If
    '    Next
    '    Throw New TimeoutException("Request timed out while waiting for " & ele_id & ". The timeout is " & seconds & " seconds.")
    'End Function

    Private Sub FreezeUI(freeze As Boolean)
        If freeze Then
            CB_Site.Enabled = False
            Btn_Query.Enabled = False
            Btn_AddAbove.Enabled = False
            Btn_AddBelow.Enabled = False
            Btn_Del.Enabled = False
            DGV.Columns("C_Serial").ReadOnly = True
            DGV.Columns("C_Model").ReadOnly = True
            'DGV.Enabled = False
        Else
            CB_Site.Enabled = True
            Btn_Query.Enabled = True
            Btn_AddAbove.Enabled = True
            Btn_AddBelow.Enabled = True
            Btn_Del.Enabled = True
            DGV.Columns("C_Serial").ReadOnly = False
            DGV.Columns("C_Model").ReadOnly = False
            'DGV.Enabled = True
        End If
    End Sub

    Private Async Sub Btn_Query_Click(sender As Object, e As EventArgs) Handles Btn_Query.Click
        If CB_Site.SelectedIndex = -1 Then
            MsgBox("Please select a website to send requests to.", MsgBoxStyle.Exclamation)
            Exit Sub
        ElseIf DGV.RowCount = 0 Then
            MsgBox("List is empty.", MsgBoxStyle.Exclamation)
            Exit Sub
        End If

        FreezeUI(True)
        Dim lst As New List(Of SerialInfo)
        For Each row As DataGridViewRow In DGV.Rows
            If row.Cells("C_Serial").Value = "" Then
                If row.Cells("C_Name").Value = "" Then
                    MsgBox("Name or serial is required for query.", MsgBoxStyle.Exclamation)
                    FreezeUI(False)
                    Exit Sub
                Else
                    Await Task.Run(
                          Sub()
                              Try
                                  Invoke(Sub() row.Cells("C_Serial").Value = "Querying...")
                                  Using wmiSearcher As New Management.ManagementObjectSearcher("\\" & row.Cells("C_Name").Value & "\root\cimv2", "select IdentifyingNumber, Name from Win32_ComputerSystemProduct")
                                      For Each inst In wmiSearcher.Get
                                          Dim ser = inst("IdentifyingNumber").ToString
                                          Dim mdl = inst("Name").ToString
                                          Invoke(Sub()
                                                     row.Cells("C_Serial").Value = ser
                                                     row.Cells("C_Model").Value = mdl
                                                     lst.Add(New SerialInfo(row.Index, row.Cells("C_Serial").Value, row.Cells("C_Model").Value))
                                                 End Sub)
                                          Exit For
                                      Next
                                  End Using
                              Catch ex As Exception
                                  Invoke(Sub() row.Cells("C_Serial").Value = "Error getting serial number." & vbCrLf & ex.Message)
                              End Try
                          End Sub)
                End If
            Else
                lst.Add(New SerialInfo(row.Index, row.Cells("C_Serial").Value, row.Cells("C_Model").Value))
            End If
        Next

        'Dim a = Query({New SerialInfo("L3EX364", "765912C"), New SerialInfo("P13P40M", "4180PLC")}, Website.Lenovo_US)
        'Query({New SerialInfo("34KRQV1", "2518B77"), New SerialInfo("34KRQV1", "291223C")}, Website.Lenovo_CN)

        'rowscast.Where(Function(r As DataGridViewRow) r.Cells(0).Value.ToString.Equals(1))
        'FreezeUI(True)

        Dim site = CB_Site.Text
        Await Task.Run(Sub() Query(lst.ToArray, [Enum].Parse(GetType(Website), site)))

        FreezeUI(False)
    End Sub

    Private Sub Btn_Exit_Click(sender As Object, e As EventArgs) Handles Btn_Exit.Click
        Me.Close()
    End Sub

    Private Sub DataGridView1_KeyDown(sender As Object, e As KeyEventArgs) Handles DGV.KeyDown
        If e.Control AndAlso e.KeyCode = Keys.V Then
            Dim currentrowi = DGV.CurrentCell.RowIndex
            Dim currentcoli = DGV.CurrentCell.ColumnIndex
            Dim data() = Clipboard.GetText().Split({vbCrLf}, StringSplitOptions.RemoveEmptyEntries)
            If DGV.RowCount - currentrowi < data.Length Then
                DGV.Rows.Add(data.Length - (DGV.RowCount - currentrowi))
            End If
            For r = 0 To data.Length - 1
                Dim dataline = data(r).Split(vbTab)
                If dataline.Length > DGV.ColumnCount - currentcoli Then
                    For c = 0 To DGV.ColumnCount - currentcoli - 1
                        DGV.Rows(currentrowi + r).Cells(currentcoli + c).Value = dataline(c)
                    Next
                Else
                    For c = 0 To dataline.Length - 1
                        DGV.Rows(currentrowi + r).Cells(currentcoli + c).Value = dataline(c)
                    Next
                End If
            Next
        ElseIf e.Control AndAlso e.KeyCode = Keys.C Then
            Dim data = DGV.GetClipboardContent()
            If data IsNot Nothing Then
                Clipboard.SetDataObject(data)
            End If
        End If
    End Sub

    Private Sub Btn_AddRow_Click(sender As Object, e As EventArgs) Handles Btn_AddAbove.Click
        If DGV.CurrentRow Is Nothing Then
            DGV.Rows.Add()
        Else
            DGV.Rows.Insert(DGV.CurrentRow.Index, 1)
        End If
        DGV.Select()
    End Sub

    Private Sub Btn_AddBelow_Click(sender As Object, e As EventArgs) Handles Btn_AddBelow.Click
        If DGV.CurrentRow Is Nothing Then
            DGV.Rows.Add()
        Else
            DGV.Rows.Insert(DGV.CurrentRow.Index + 1, 1)
        End If
        DGV.Select()
    End Sub

    Private Sub WarrantyQuery_Load(sender As Object, e As EventArgs) Handles Me.Load
        DGV.Select()
        DGV.Rows.Add()
        CB_Site.Items.AddRange([Enum].GetNames(GetType(Website)))
        For Each col As DataGridViewColumn In DGV.Columns
            If col.ReadOnly Then col.DefaultCellStyle.BackColor = Color.LightGray
        Next

        AddHandler SingleQueryReturned,
            Sub(_wi As WarrantyInfo)
                Invoke(Sub()
                           Dim row = DGV.Rows(_wi.target.id)
                           row.Cells("C_Start").Value = _wi.start_date
                           row.Cells("C_End").Value = _wi.end_date
                           row.Cells("C_Raw").Value = _wi.raw_str
                           DGV.Refresh()
                       End Sub)
            End Sub
    End Sub

    Private Sub Btn_Del_Click(sender As Object, e As EventArgs) Handles Btn_Del.Click
        Dim tmplst As New List(Of DataGridViewRow)
        For Each c As DataGridViewCell In DGV.SelectedCells
            If Not tmplst.Contains(c.OwningRow) Then tmplst.Add(c.OwningRow)
        Next
        For Each row In tmplst
            DGV.Rows.Remove(row)
        Next
    End Sub

    'deprecated method for Dell
    '{"Dell_US", "http://www.dell.com/support/home/us/en/19/product-support/servicetag/<>/warranty"}
    'Using wb As New WebBrowser
    '    wb.ScriptErrorsSuppressed = True
    '    For Each _si In si
    '        Try
    '            wb.Navigate(SiteAddress(s.ToString).Replace("<>", _si.serial))
    '            WaitForElement(wb, "hrefWarrantyID", timeout, False)
    '            Application.DoEvents() 'this is to wait for the page to load fully

    '            If Not IsNothing(wb.Document.GetElementById("RegistrationCaptcha_CaptchaImage")) Then
    '                Using CaptchaPrompt As New Form
    '                    'this is from http://stackoverflow.com/questions/15736291/copy-image-from-webbrowser-to-clipboard
    '                    'for getting a copy of the captcha to clipboard
    '                    Dim doc As mshtml.IHTMLDocument2 = DirectCast(wb.Document.DomDocument, mshtml.IHTMLDocument2)
    '                    Dim body As mshtml.IHTMLElement2 = DirectCast(doc.body, mshtml.IHTMLElement2)
    '                    Dim imgRange As mshtml.IHTMLControlRange = DirectCast(body.createControlRange(), mshtml.IHTMLControlRange)
    '                    Dim image As mshtml.IHTMLControlElement = DirectCast(DirectCast(doc, mshtml.IHTMLDocument3).getElementById("RegistrationCaptcha_CaptchaImage"), mshtml.IHTMLControlElement)
    '                    imgRange.add(image)
    '                    imgRange.execCommand("Copy", False, Nothing)

    '                    With CaptchaPrompt
    '                        .Text = "Enter Captcha"
    '                        .Size = New Drawing.Size(300, 200)
    '                        .FormBorderStyle = Windows.Forms.FormBorderStyle.FixedToolWindow
    '                        .ControlBox = False
    '                        .ShowInTaskbar = False
    '                        '.Font = New Font("微软雅黑", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
    '                        '.Size = New Size(250, 150)
    '                        .StartPosition = FormStartPosition.CenterScreen
    '                    End With
    '                    Dim PB_Captcha As New PictureBox
    '                    With PB_Captcha
    '                        .Parent = CaptchaPrompt
    '                        .SizeMode = PictureBoxSizeMode.Zoom
    '                        .Size = New Drawing.Size(260, 90)
    '                        .Location = New Drawing.Point(12, 12)
    '                        .Image = Clipboard.GetImage
    '                        '.ImageLocation = wb.Document.GetElementById("RegistrationCaptcha_CaptchaImage").GetAttribute("src")
    '                    End With
    '                    Clipboard.Clear()
    '                    Dim TB_Captcha As New TextBox
    '                    With TB_Captcha
    '                        .Parent = CaptchaPrompt
    '                        .Location = New Drawing.Point(12, 114)
    '                        .Width = 260
    '                    End With
    '                    AddHandler TB_Captcha.KeyDown, Sub(sender As Object, e As KeyEventArgs)
    '                                                       If e.KeyCode = Keys.Enter Then
    '                                                           wb.Document.GetElementById("CaptchaCode").SetAttribute("value", TB_Captcha.Text)
    '                                                           wb.Document.GetElementById("btnSubmitCaptchawarranty").InvokeMember("Click")
    '                                                           CaptchaPrompt.DialogResult = Windows.Forms.DialogResult.OK
    '                                                       End If
    '                                                   End Sub
    '                    CaptchaPrompt.ShowDialog()
    '                End Using
    '            End If
    '            'above is for the captcha

    '            Dim html = WaitForElement(wb, "printdivid", 10, False).InnerHtml
    '            html = html.Substring(html.IndexOf("<tbody>"))
    '            html = html.Remove(html.IndexOf("</tbody>") + 8)
    '            Dim xele = XElement.Parse(html)
    '            Dim tmpstr As String, startdate = Date.MaxValue, enddate = Date.MinValue
    '            For Each ele In xele.Elements
    '                If ele.Elements().Count > 3 Then
    '                    Dim tmpdate = DateTime.Parse(ele.Elements()(2).Value.Trim)
    '                    If startdate > tmpdate Then startdate = tmpdate
    '                    tmpdate = DateTime.Parse(ele.Elements()(3).Value.Trim)
    '                    If enddate < tmpdate Then enddate = tmpdate

    '                    tmpstr += ele.Elements()(0).Value.Trim & vbCrLf & "     From " & ele.Elements()(2).Value.Trim & " To " & ele.Elements()(3).Value.Trim & vbCrLf
    '                End If
    '            Next
    '            Dim winfo = New WarrantyInfo(_si, If(startdate = Date.MaxValue, "", startdate.ToString("yyyy-MM-dd")), If(enddate = Date.MinValue, "", enddate.ToString("yyyy-MM-dd")), tmpstr)
    '            infolist.Add(winfo)
    '            RaiseEvent SingleQueryReturned(winfo)
    '            wb.Navigate("about:blank")
    '            Application.DoEvents()
    '        Catch ex As Exception
    '            Dim winfo = New WarrantyInfo(_si, "", "", ex.Message)
    '            infolist.Add(winfo)
    '            RaiseEvent SingleQueryReturned(winfo)
    '        End Try
    '    Next
    'End Using
End Class