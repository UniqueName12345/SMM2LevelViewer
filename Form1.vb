Imports System.Drawing.Drawing2D
Imports System.Reflection
Imports System
Imports System.Diagnostics
Imports System.Net
Imports System.IO
Imports Newtonsoft.Json.Linq

Public Class Form1
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        isMapIO = True
        RefPic()
        Button2.Enabled = True
    End Sub
    Private Sub Button6_Click(sender As Object, e As EventArgs)
        isMapIO = False
        LoadEFile(False)
        InitPng()
        DrawObject(False)

        Form2.P.Image = B
        Button2.Enabled = True
    End Sub
    Public Sub LoadEFile(IO As Boolean)
        'Level file header H00 length 200
        LoadLvlData(TextBox1.Text, IO)
        If IO Then
            Label2.Text += vbCrLf & vbCrLf
            Label2.Text += "image name:" & LH.Name & vbCrLf
            Label2.Text += "describe:" & LH.Desc & vbCrLf
            Label2.Text += "time:" & LH.Timer & vbCrLf
            Label2.Text += "style:" & LH.GameStyle & vbCrLf
            Label2.Text += "Version:" & LH.GameVer & vbCrLf
            Label2.Text += "starting point:" & LH.StartY & vbCrLf
            Label2.Text += "end:" & LH.GoalX & "," & LH.GoalY & vbCrLf
            Label2.Text += "======Table World======" & vbCrLf
            Label2.Text += "theme:" & MapHdr.Theme & vbCrLf
            Label2.Text += "width:" & MapHdr.BorR & vbCrLf
            Label2.Text += "high:" & MapHdr.BorT & vbCrLf
            Label2.Text += "Bricks:" & MapHdr.GroundCount & vbCrLf
            Label2.Text += "unit:" & MapHdr.ObjCount & vbCrLf
            Label2.Text += "track:" & MapHdr.TrackCount & vbCrLf
            Label2.Text += "Scroll:" & MapHdr.AutoscrollType & vbCrLf
            Label2.Text += "Water surface:" & MapHdr.LiqSHeight & "-" & MapHdr.LiqEHeight & vbCrLf
        Else
            Label2.Text += "====== 里世界======" & vbCrLf
            Label2.Text += "Theme:" & MapHdr.Theme & vbCrLf
            Label2.Text += "Width:" & MapHdr.BorR & vbCrLf
            Label2.Text += "Height:" & MapHdr.BorT & vbCrLf
            Label2.Text += "Brick:" & MapHdr.GroundCount & vbCrLf
            Label2.Text += "Unit:" & MapHdr.ObjCount & vbCrLf
            Label2.Text += "Track:" & MapHdr.TrackCount & vbCrLf
            Label2.Text += "Scroll:" & MapHdr.AutoscrollType & vbCrLf
            Label2.Text += "Water surface:" & MapHdr.LiqSHeight & "-" & MapHdr.LiqEHeight & vbCrLf
        End If
        Dim LInfo() As FieldInfo
        Dim I As FieldInfo
        If IO Then
            LInfo = LH.GetType.GetFields()
            TextBox2.Text = ""
            For Each I In LInfo
                TextBox2.Text += I.Name & ":" & I.GetValue(LH) & vbCrLf
            Next
            TextBox3.Text = "===M0===" & vbCrLf
            'Watch world H200 length 2DEE0
            LInfo = MapHdr.GetType.GetFields()
            For Each I In LInfo
                TextBox3.Text += I.Name & ":" & I.GetValue(MapHdr).ToString & vbCrLf
            Next
        Else
            TextBox4.Text = "===M1===" & vbCrLf
            'Watch world H200 length 2DEE0
            LInfo = MapHdr.GetType.GetFields()
            For Each I In LInfo
                TextBox4.Text += I.Name & ":" & I.GetValue(MapHdr).ToString & vbCrLf
            Next
        End If


    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Form2.Show()
    End Sub
    Dim B As Bitmap
    Dim G As Graphics
    Dim Zm As Integer
    Public Sub InitPng()
        Dim i As Integer
        Dim H As Integer, W As Integer
        H = MapHdr.BorT \ 16
        W = MapHdr.BorR \ 16
        Zm = 2 ^ TrackBar1.Value
        B = New Bitmap(W * Zm, H * Zm)
        G = Graphics.FromImage(B)
        Form2.P.Width = W * Zm
        Form2.P.Height = H * Zm
        G.InterpolationMode = InterpolationMode.NearestNeighbor

        For i = 0 To H
            G.DrawLine(Pens.LightGray, 0, CSng(i * Zm), CSng(W * Zm), CSng(i * Zm))
            If i Mod 13 = 0 Then
                G.DrawLine(Pens.LightGray, 0, CSng((H-i) * Zm + 1), CSng(W * Zm), CSng((H-i) * Zm + 1))
            End If
            If (H-i) Mod 10 = 0 Then
                G.DrawString((H-i).ToString, Button1.Font, Brushes.Black, 0, CSng(i * Zm))
            End If
        Next
        For i = 0 To W
            G.DrawLine(Pens.LightGray, CSng(i * Zm), 0, CSng(i * Zm), CSng(H * Zm))
            If i Mod 24 = 0 Then
                G.DrawLine(Pens.LightGray, CSng(i * Zm + 1), 0, CSng(i * Zm + 1), CSng(H * Zm))
            End If
            If i Mod 10 = 9 Then
                G.DrawString((i + 1).ToString, Button1.Font, Brushes.Black, CSng(i * Zm), 0)
            End If
        Next
        Dim BC1, BC2 As Color
        If MapHdr.Theme = 2 Then
            BC1 = Color.FromArgb(64, 255, 0, 0)
            BC2 = Color.FromArgb(64, 255, 0, 0)
            G.FillRectangle(New SolidBrush(BC2), 0, CSng((H-MapHdr.LiqEHeight-0.5) * Zm), CSng(W * Zm), CSng(H * Zm))
            G.FillRectangle(New SolidBrush(BC1), 0, CSng((H-MapHdr.LiqSHeight-0.5) * Zm), CSng(W * Zm),CSng(H * Zm))
        ElseIf MapHdr.Theme = 9 Then
            BC1 = Color.FromArgb(64, 0, 0, 255)
            BC2 = Color.FromArgb(64, 0, 0, 255)
            G.FillRectangle(New SolidBrush(BC2), 0, CSng((H-MapHdr.LiqEHeight-0.5) * Zm), CSng(W * Zm), CSng(H * Zm))
            G.FillRectangle(New SolidBrush(BC1), 0, CSng((H-MapHdr.LiqSHeight-0.5) * Zm), CSng(W * Zm), CSng(H * Zm))
        End If

    End Sub
    Public Sub InitPng2()
        'ReDim OBJ(ObjEng.GetUpperBound(0))
        Dim i As Integer
        'For i = 0 To OBJ.GetUpperBound(0)
        'Try
        'OBJ(i) = New Bitmap(PT & "\img\" & LH.GameStyle.ToString & "\obj\" & i.ToString & ".PNG")
        'Catch
        'End Try
        'Next
        'FLG(0) = New Bitmap(PT & "\img\" & LH.GameStyle.ToString & "\obj\F0.PNG")
        'FLG(1) = New Bitmap(PT & "\img\" & LH.GameStyle.ToString & "\obj\F1.PNG")
        'FLG(2) = New Bitmap(PT & "\img\" & LH.GameStyle.ToString & "\obj\F2.PNG")

        Dim H As Integer, W As Integer
        H = MapHdr.BorT \ 16
        W = MapHdr.BorR \ 16
        Zm = 2 ^ TrackBar1.Value
        B = New Bitmap(W * Zm, H * Zm)
        G = Graphics.FromImage(B)
        Form3.P.Width = W * Zm
        Form3.P.Height = H * Zm
        G.InterpolationMode = InterpolationMode.NearestNeighbor
        For i = 0 To H
            G.DrawLine(Pens.WhiteSmoke, 0, CSng(i * Zm), CSng(W * Zm), CSng(i * Zm))
            If i Mod 13 = 0 Then
                G.DrawLine(Pens.WhiteSmoke, 0, CSng((H-i) * Zm + 1), CSng(W * Zm), CSng((H-i) * Zm + 1))
            End If
            If (H-i) Mod 10 = 0 Then
                G.DrawString((H-i).ToString, Button1.Font, Brushes.Black, 0, CSng(i * Zm))
            End If
        Next
        For i = 0 To W
            G.DrawLine(Pens.WhiteSmoke, CSng(i * Zm), 0, CSng(i * Zm), CSng(H * Zm))
            If i Mod 24 = 0 Then
                G.DrawLine(Pens.WhiteSmoke, CSng(i * Zm + 1), 0, CSng(i * Zm + 1), CSng(H * Zm))
            End If
            If i Mod 10 = 9 Then
                G.DrawString((i + 1).ToString, Button1.Font, Brushes.Black, CSng(i * Zm), 0)
            End If
        Next

        Dim BC1, BC2 As Color
        If MapHdr.Theme = 2 Then
            BC1 = Color.FromArgb(64, 255, 0, 0)
            BC2 = Color.FromArgb(64, 255, 0, 0)
            G.FillRectangle(New SolidBrush(BC2), 0, CSng((H-MapHdr.LiqEHeight-0.5) * Zm), CSng(W * Zm), CSng(H * Zm))
            G.FillRectangle(New SolidBrush(BC1), 0, CSng((H-MapHdr.LiqSHeight-0.5) * Zm), CSng(W * Zm), CSng(H * Zm))
        ElseIf MapHdr.Theme = 9 Then
            BC1 = Color.FromArgb(64, 0, 0, 255)
            BC2 = Color.FromArgb(64, 0, 0, 255)
            G.FillRectangle(New SolidBrush(BC2), 0, CSng((H-MapHdr.LiqEHeight-0.5) * Zm), CSng(W * Zm), CSng(H * Zm))
            G.FillRectangle(New SolidBrush(BC1), 0, CSng((H-MapHdr.LiqSHeight-0.5) * Zm), CSng(W * Zm), CSng(H * Zm))
        End If

    End Sub
    Public Sub DrawMoveBlock(ID As Byte, EX As Byte, X As Integer, Y As Integer)
        On Error GoTo Err
        Dim H, W, XX, YY As Integer
        H = MapHdr.BorT \ 16
        W = MapHdr.BorR \ 16
        XX = X / 160 + 1
        YY = (Y + 80) / 160 + 1
        Dim i As Integer

        Select Case ID
            Case 85
                Select Case MapTrackBlk(EX-1).Node(0).p1
                    Case 1, 5, 7, 14
                        XX -= 4
                    Case 2, 9, 11, 13

                    Case 3, 6, 10, 16
                        XX -= 2
                        YY -= 2
                    Case 4, 8, 12, 15
                        XX -= 2
                        YY += 2
                End Select
                For i = 0 To MapTrackBlk(EX-1).NodeCount-1
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SS.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                    'G.DrawString(MapTrackBlk(EX-1).Node(i).p1, Me.Font, Brushes.Black, (XX) * Zm, (H-YY) * Zm)
                    Select Case MapTrackBlk(EX-1).Node(i).p1
                        Case 1'L
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SL.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            XX -= 2
                        Case 2'R
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SR.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            XX += 2
                        Case 3'D
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SD.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            YY -= 2
                        Case 4'U
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SU.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            YY += 2
                        Case 5'LD
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SRD.PNG"),
 XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            YY -= 2Case 6'DL
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SUL.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            XX -= 2
                        Case 7'LU
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SRU.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            YY += 2
                        Case 8'UL
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SDL.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            XX -= 2
                        Case 9'RD
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SLD.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            YY -= 2
                        Case 10'DR
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SUR.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            XX += 2
                        Case 11'RU
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SLU.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            YY += 2
                        Case 12'UR
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SDR.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            XX += 2
                        Case 13'RE
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SE.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                        Case 14'LE
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SE.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                        Case 15'UE
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SE.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                        Case 16'DE
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SE.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                    End Select
                Next
            Case 119
                Select Case MapMoveBlk(EX-1).Node(0).p1
                    Case 1, 5, 7, 14
                        XX -= 4
                    Case 2, 9, 11, 13

                    Case 3, 6, 10, 16
                        XX -= 2
                        YY -= 2
                    Case 4, 8, 12, 15
                        XX -= 2
                        YY += 2
                End Select
                For i = 0 To MapMoveBlk(EX-1).NodeCount-1
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SS.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                    'G.DrawString(MapMoveBlk(EX-1).Node(i).p1, Me.Font, Brushes.Black, (XX) * Zm, (H-YY) * Zm)
                    Select Case MapMoveBlk(EX-1).Node(i).p1
                        Case 1'L
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SL.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            XX -= 2
                        Case 2'R
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SR.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            XX += 2
                        Case 3'D
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SD.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            YY -= 2
                        Case 4'U
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SU.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            YY += 2
                        Case 5'LD
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SRD.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            YY -= 2
                        Case 6'DL
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SUL.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            XX -= 2
                        Case 7'LU
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SRU.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            YY += 2
                        Case 8'UL
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SDL.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            XX -= 2
                        Case 9'RD
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SLD.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            YY -= 2
                        Case 10'DR
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SUR.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            XX += 2
                        Case 11'RU
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SLU.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            YY += 2
                        Case 12'UR
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SDR.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                            XX += 2
                        Case 13'RE
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SE.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                        Case 14'LE
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SE.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                        Case 15'UE
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SE.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                        Case 16'DE
                            G.DrawImage(Image.FromFile(PT & "\img\CMN\SE.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                    End Select
                Next
        End Select
Err:
    End Sub
    Public Sub DrawCrp(EX As Byte, X As Integer, Y As Integer)
        On Error GoTo Err
        Dim H, W, XX, YY As Integer
        H = MapHdr.BorT \ 16
        W = MapHdr.BorR \ 16
        XX = X / 160 + 1
        YY = (Y + 80) / 160 + 1
        Dim i As Integer

        Select Case MapCrp(EX-1).Node(0)
            Case 1, 5, 7, 14
                XX -= 4
            Case 2, 9, 11, 13

            Case 3, 6, 10, 16
                XX -= 2
                YY -= 2
            Case 4, 8, 12, 15
                XX -= 2
                YY += 2
        End Select

        For i = 0 To MapCrp(EX-1).NodeCount-1
            G.DrawImage(Image.FromFile(PT & "\img\CMN\SS.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
            'G.DrawString(MapCrp(EX-1).Node(i), Me.Font, Brushes.Black, (XX) * Zm, (H-YY) * Zm)
            Select Case MapCrp(EX-1).Node(i)
                Case 1'L
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SL.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                    XX -= 2
                Case 2'R
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SR.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                    XX += 2
                Case 3'D
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SD.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                    YY -= 2
                Case 4'U
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SU.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                    YY += 2
                Case 5'LD
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SRD.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                    YY -= 2
                Case 6'DL
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SUL.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                    XX -= 2
                Case 7'LU
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SRU.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                    YY += 2
                Case 8'UL
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SDL.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                    XX -= 2
                Case 9'RD
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SLD.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                    YY -= 2
                Case 10'DR
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SUR.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                    XX += 2
                Case 11'RU
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SLU.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                    YY += 2
                Case 12'UR
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SDR.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                    XX += 2
                Case 13'RE
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SE.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                Case 14'LE
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SE.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                Case 15'UE
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SE.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
                Case 16'DE
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SE.PNG"), XX * Zm, (H-YY) * Zm, Zm * 2, Zm * 2)
            End Select
        Next
Err:
    End Sub
    Public Sub DrawSnake(EX As Byte, X As Integer, Y As Integer, SW As Integer, SH As Integer)
        'Snake Bricks
        On Error GoTo Err
        Dim H, W, XX, YY As Integer
        H = MapHdr.BorT \ 16
        W = MapHdr.BorR \ 16

        YY = (Y + SH * 80) / 160
        If EX <&H10 Then
            XX = (X + SW * 80) / 160
            EX = EX Mod &H10
            Select Case MapSnk(EX-1).Node(0).Dir
                Case 1, 5, 7
                    XX -= 1
                Case 2, 9, 11

                Case 3, 6, 10
   
                 XX -= 1
                    YY -= 1
                Case 4, 8, 12
                    XX -= 1
                    YY += 1
            End Select
        ElseXX = (X-SW * 80) / 160
            EX = EX Mod &H10
            Select Case MapSnk(EX-1).Node(0).Dir
                Case 1, 5, 7
                    XX -= 1
                Case 2, 9, 11

                Case 3, 6, 10
                    YY -= 1
                Case 4, 8, 12
                    YY += 1
            End Select
        End If


        Dim i As Integer


        For i = 0 To MapSnk(EX-1).NodeCount-1
            G.DrawImage(Image.FromFile(PT & "\img\CMN\SS.PNG"), XX * Zm, (H-YY) * Zm, Zm, Zm)
            'G.DrawString(MapSnk(EX-1).Node(i).Dir, Me.Font, Brushes.Black, (XX + 0.5) * Zm, (H-YY-0.5) * Zm)
            Select Case MapSnk(EX-1).Node(i).Dir
                Case 1'L
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SL.PNG"), XX * Zm, (H-YY) * Zm, Zm, Zm)
                    XX -= 1
                Case 2'R
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SR.PNG"), XX * Zm, (H-YY) * Zm, Zm, Zm)
                    XX += 1
                Case 3'D
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SD.PNG"), XX * Zm, (H-YY) * Zm, Zm, Zm)
                    YY -= 1
                Case 4'U
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SU.PNG"), XX * Zm, (H-YY) * Zm, Zm, Zm)
                    YY += 1
                Case 5'LD
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SRD.PNG"), XX * Zm, (H-YY) * Zm, Zm, Zm)
                    YY -= 1
                Case 6'DL
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SUL.PNG"), XX * Zm, (H-YY) * Zm, Zm, Zm)
                    XX -= 1
                Case 7'LU
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SRU.PNG"), XX * Zm, (H-YY) * Zm, Zm, Zm)
                    YY += 1
                Case 8'UL
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SDL.PNG"), XX * Zm, (H-YY) * Zm, Zm, Zm)
                    XX -= 1
                Case 9'RD
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SLD.PNG"), XX * Zm, (H-YY) * Zm, Zm, Zm)
                    YY -= 1
                Case 10'DR
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SUR.PNG"), XX * Zm, (H-YY) * Zm, Zm, Zm)
                    XX += 1
                Case 11'RU
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SLU.PNG"), XX * Zm, (H-YY) * Zm, Zm, Zm)
                    YY += 1
                Case 12'UR
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SDR.PNG"), XX * Zm, (H-YY) * Zm, Zm, Zm)
                    XX += 1
                Case 13'RE
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SE.PNG"), XX * Zm, (H-YY) * Zm, Zm, Zm)
                Case 14'LE
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SE.PNG"), XX * Zm, (H-YY) * Zm, Zm, Zm)
                Case 15'UE
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SE.PNG"), XX * Zm, (H-YY) * Zm, Zm, Zm)
                Case 16'DE
                    G.DrawImage(Image.FromFile(PT & "\img\CMN\SE.PNG"), XX * Zm, (H-YY) * Zm, Zm, Zm)
            End Select

        Next

Err:
    End Sub
    Public Sub DrawIce()
        'Ice cubes
        Dim i As Integer, H As Integer
        For i = 0 To MapHdr.IceCount-1
            If MapIce(i).ID = 0 Then
                G.DrawImage(GetTile(15, 41, 1, 2), MapIce(i).X * Zm, (MapHdr.BorT \ 16-2) * Zm-MapIce(i).Y * Zm, Zm, Zm * 2 )
                For H = 1 To 2
                    ObjLocData(NowIO, MapIce(i).X + 1, Int(H + MapIce(i).Y)).Obj += "118,"
                    ObjLocData(NowIO, MapIce(i).X + 1, Int(H + MapIce(i).Y)).Flag += ","
                    ObjLocData(NowIO, MapIce(i).X + 1, Int(H + MapIce(i).Y)).SubObj += ","
                    ObjLocData(NowIO, MapIce(i).X + 1, Int(H + MapIce(i).Y)).SubFlag += ","
                    ObjLocData(NowIO, MapIce(i).X + 1, Int(H + MapIce(i).Y)).State += ","
                    ObjLocData(NowIO, MapIce(i).X + 1, Int(H + MapIce(i).Y)).SubState += ","
                Next
            Else
                G.DrawImage(GetTile(15, 39, 1, 2), MapIce(i).X * Zm, (MapHdr.BorT \ 16-2) * Zm-MapIce(i).Y * Zm, Zm, Zm * 2 )
                For H = 1 To 2
                    ObjLocData(NowIO, MapIce(i).X + 1, Int(H + MapIce(i).Y)).Obj += "118A,"
                    ObjLocData(NowIO, MapIce(i).X + 1, Int(H + MapIce(i).Y)).Flag += ","
                    ObjLocData(NowIO, MapIce(i).X + 1, Int(H + MapIce(i).Y)).SubObj += ","
                    ObjLocData(NowIO, MapIce(i).X + 1, Int(H + MapIce(i).Y)).SubFlag += ","
                    ObjLocData(NowIO, MapIce(i).X + 1, Int(H + MapIce(i).Y)).State += ","
                    ObjLocData(NowIO, MapIce(i).X + 1, Int(H + MapIce(i).Y)).SubState += ","
                Next
            End If
        Next
    End Sub
    Public Sub DrawTrack()
        'track
        Dim H As Integer, W As Integer
        H = MapHdr.BorT \ 16
        W = MapHdr.BorR \ 16Dim i As Integer
        For i = 0 To MapHdr.TrackCount-1
            'LID+1?
            ObjLinkType(MapTrk(i).LID) = 59
            If MapTrk(i).Type <8 Then
                G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\T" & MapTrk(i).Type.ToString & ".PNG"), MapTrk(i).X * Zm-Zm, (H-2) * Zm-MapTrk(i).Y * Zm, Zm * 3, Zm * 3)
                Select Case MapTrk(i).Type
                    Case 0
                        If TrackNode(MapTrk(i).X + 1 + 1, MapTrk(i).Y + 1) = 1 AndAlso MapTrk(i).F0 = 0 Then
                            G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\T.PNG"), MapTrk(i).X * Zm + Zm, (H-1) * Zm -MapTrk(i).Y * Zm, Zm, Zm)
                        End If
                        If TrackNode(MapTrk(i).X + 1-1, MapTrk(i).Y + 1) = 1 AndAlso MapTrk(i).F1 = 0 Then
                            G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\T.PNG"), MapTrk(i).X * Zm-Zm, (H-1) * Zm -MapTrk(i).Y * Zm, Zm, Zm)
                        End If
                    Case 1
                        If TrackNode(MapTrk(i).X + 1, MapTrk(i).Y + 1 + 1) = 1 AndAlso MapTrk(i).F0 = 0 Then
                            G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\T.PNG"), MapTrk(i).X * Zm, (H-2) * Zm-MapTrk (i).Y * Zm, Zm, Zm)
                        End If
                        If TrackNode(MapTrk(i).X + 1, MapTrk(i).Y + 1-1) = 1 AndAlso MapTrk(i).F1 = 0 Then
                            G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\T.PNG"), MapTrk(i).X * Zm, H * Zm-MapTrk(i). Y * Zm, Zm, Zm)
                        End If
                    Case 2, 4, 5
                        If TrackNode(MapTrk(i).X + 1 + 1, MapTrk(i).Y + 1-1) = 1 AndAlso MapTrk(i).F0 = 0 Then
                            G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\T.PNG"), MapTrk(i).X * Zm + Zm, H * Zm-MapTrk(i ).Y * Zm, Zm, Zm)
                        End If
                        If TrackNode(MapTrk(i).X + 1-1, MapTrk(i).Y + 1 + 1) = 1 AndAlso MapTrk(i).F1 = 0 Then
                            G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\T.PNG"), MapTrk(i).X * Zm-Zm, (H-2) * Zm -MapTrk(i).Y * Zm, Zm, Zm)
                        End If
                    Case 3, 6, 7
                        If TrackNode(MapTrk(i).X + 1 + 1, MapTrk(i).Y + 1 + 1) = 1 AndAlso MapTrk(i).F0 = 0 Then
                            G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\T.PNG"), MapTrk(i).X * Zm + Zm, (H-2) * Zm -MapTrk(i).Y * Zm, Zm, Zm)
                        End If
                        If TrackNode(MapTrk(i).X + 1-1, MapTrk(i).Y + 1-1) = 1 AndAlso MapTrk(i).F1 = 0 Then
                            G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\T.PNG"), MapTrk(i).X * Zm-Zm, H * Zm-MapTrk(i ).Y * Zm, Zm, Zm)
                        End If
                End Select
            Else'Y track
                G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\T" & MapTrk(i).Type.ToString & ".PNG"), MapTrk(i).X * Zm-Zm, (H-4) * Zm-MapTrk(i).Y * Zm, Zm * 5, Zm * 5)
                'G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\T.PNG"), (MapTrk(i).X-1 + TrackYPt(MapTrk(i). Type, 0).X) * Zm, H * Zm-(MapTrk(i).Y + TrackYPt(MapTrk(i).Type, 0).Y) * Zm, Zm, Zm)
                'G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\T.PNG"), (MapTrk(i).X-1 + TrackYPt(MapTrk(i). Type, 1).X) * Zm, H * Zm-(MapTrk(i).Y + TrackYPt(MapTrk(i).Type, 1).Y) * Zm, Zm, Zm)
                'G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\T.PNG"), (MapTrk(i).X-1 + TrackYPt(MapTrk(i). Type, 2).X) * Zm, H * Zm-(MapTrk(i).Y + TrackYPt(MapTrk(i).Type, 2).Y) * Zm, Zm, Zm)

                If TrackNode(MapTrk(i).X + TrackYPt(MapTrk(i).Type, 0).X, MapTrk(i).Y + TrackYPt(MapTrk(i).Type, 0).Y) = 1 AndAlso MapTrk( i).F0 = 0 Then
                    G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\T.PNG"), (MapTrk(i).X-1 + TrackYPt(MapTrk(i).Type , 0).X) * Zm, (H-4) * Zm-(MapTrk(i).Y-TrackYPt(MapTrk(i).Type, 0).Y) * Zm, Zm, Zm)
                End If
                If TrackNode(MapTrk(i).X + TrackYPt(MapTrk(i).Type, 1).X, MapTrk(i).Y + TrackYPt(MapTrk(i).Type, 1).Y) = 1 AndAlso MapTrk( i).F1 = 0 Then
                    G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\T.PNG"), (MapTrk(i).X-1 + TrackYPt(MapTrk(i).Type , 1).X) * Zm, (H-4) * Zm-(MapTrk(i).Y-TrackYPt(MapTrk(i).Type, 1).Y)
 * Zm, Zm, Zm)
                End If
                If TrackNode(MapTrk(i).X + TrackYPt(MapTrk(i).Type, 2).X, MapTrk(i).Y + TrackYPt(MapTrk(i).Type, 2).Y) = 1 AndAlso MapTrk( i).F2 = 0 Then
                    G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\T.PNG"), (MapTrk(i).X-1 + TrackYPt(MapTrk(i).Type, 2).X) * Zm, (H-4) * Zm-(MapTrk(i).Y- TrackYPt(MapTrk(i).Type, 2).Y) * Zm, Zm, Zm)
                End If
            End If
        Next
    End Sub
    Public Sub DrawSlope()
        Dim H As Integer, W As Integer
        H = MapHdr.BorT \ 16
        W = MapHdr.BorR \ 16
        Dim i As Integer
        Dim CX, CY As Integer
        'GrdType
        '0 None 1 square 2 steep upper left 3 steep upper right 4 steep lower left 5 steep lower right 6 end upper left 7 end upper right 8 end lower left 9 end lower right
        '
        For i = 0 To MapHdr.ObjCount-1
            Select Case MapObj(i).ID
                Case 87
                    'Gentle slope
                    CX = (-0.5 + MapObj(i).X / 160)
                    CY = (-0.5 + MapObj(i).Y / 160)
                    If (MapObj(i).Flag \ &H100000) Mod &H2 = 0 Then
                        'Left oblique
                        G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\7.PNG"),
                                            CSng(CX * Zm), (H-1) * Zm-CSng(CY * Zm), Zm, Zm)
                        G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\7.PNG"),
                                            CSng((MapObj(i).W-1.5 + MapObj(i).X / 160) * Zm), (H-1) * Zm-CSng((CY + MapObj(i).H-1) * Zm) , Zm, Zm)
                        'G.DrawString(GroundNode(CX + 1, CY + 1), Me.Font, Brushes.Black, CSng(CX * Zm), (H-1) * Zm-CSng(CY * Zm))
                        'G.DrawString(GroundNode(CX + MapObj(i).W, CY + MapObj(i).H), Me.Font, Brushes.Black, CSng((CX + MapObj(i).W-1) * Zm ), (H-1) * Zm-CSng((CY + MapObj(i).H-1) * Zm))

                        For j = 1 To MapObj(i).W-2 Step 2
                            G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\87.PNG"),
                                                CSng((CX + j) * Zm),
                                                (H-1) * Zm-CSng((j / 2 + MapObj(i).Y / 160) * Zm), Zm * 2, Zm * 2)
                            'G.DrawString(GroundNode(CX + 1 + j, CY + 3 + (j \ 2)), Me.Font, Brushes.Black, CSng((CX + j) * Zm), (H-1) * Zm -CSng((CY + 1 + (j \ 2)) * Zm))
                            'G.DrawString(GroundNode(CX + 1 + j, CY + 2 + (j \ 2)), Me.Font, Brushes.Black, CSng((CX + j) * Zm), (H-1) * Zm -CSng((CY + (j \ 2)) * Zm))

                            'G.DrawString(GroundNode(CX + 2 + j, CY + 3 + (j \ 2)), Me.Font, Brushes.Black, CSng((CX + j + 1) * Zm), (H-1) * Zm-CSng((CY + 1 + (j \ 2)) * Zm))
                            'G.DrawString(GroundNode(CX + 2 + j, CY + 2 + (j \ 2)), Me.Font, Brushes.Black, CSng((CX + j + 1) * Zm), (H-1) * Zm-CSng((CY + (j \ 2)) * Zm))
                        Next
                    Else
                        'Right diagonal
                        G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\7.PNG"),
                                            CSng((-0.5 + MapObj(i).X / 160) * Zm),
                                            (H-1) * Zm-CSng((MapObj(i).H-1.5 + MapObj(i).Y / 160) * Zm), Zm, Zm)
                        G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\7.PNG"),
                                            CSng((MapObj(i).W-1.5 + MapObj(i).X / 160) * Zm),
                                            (H-1) * Zm-CSng((-0.5 + MapObj(i).Y / 160) * Zm), Zm, Zm)
                        'G.DrawString(GroundNode(CX + 1, CY + MapObj(i).H), Me.Font, Brushes.Black, CSng(CX * Zm), (H-1) * Zm-CSng((CY + MapObj (i).H-1) * Zm))
                        'G.DrawString(GroundNode(CX + MapObj(i).W, CY + 1), Me.Font, Brushes.Black, CSng((CX + MapObj(i).W-1) * Zm), (H- 1) * Zm-CSng(CY * Zm))
                        For j = 1 To MapObj(i).W-2 Step 2

                            G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\87A.PNG"),
                                                CSng((j-0.5 + MapObj(i).X / 160) * Zm),
                                                (H-1) * Zm-CSng((-j / 2 + MapObj(i).H-1 + MapObj(i).Y / 160) * Zm), Zm * 2, Zm * 2)
                            'G.DrawString(GroundNode(CX + 1 + j, CY + MapObj(i).H-(j \ 2)), Me.Font, Brushes.Black,
                            'CSng((CX + j) * Zm), (H-1) * Zm-CSng((CY + MapObj(i).H-(j \ 2)-1) * Zm))
                            'G.DrawString(GroundNode(CX + 1 + j, CY + MapObj(i).H-(j \ 2)-1), Me.Font, Brushes.Black,
                            'CSng((CX + j) * Zm), (H-1) * Zm-CSng((CY + MapObj(i).H-(j \ 2)-2) * Zm))

                            'G.DrawString(GroundNode(CX + 2 + j, CY + MapObj(i).H-(j \ 2)), Me.Font, Brushes.Black,
                            'CSng((CX + j + 1) * Zm), (H-1) * Zm-CSng((CY + MapObj(i).H-(j \ 2)-1) * Zm))
                            'G.DrawString(GroundNode(CX + 2 + j, CY + MapObj(i).H-(j \ 2)-1), Me.Font, Brushes.Black,
                            'CSng((CX + j + 1) * Zm), (H-1) * Zm-CSng((CY + MapObj(i).H-(j \ 2)-2) * Zm))
                        Next


                    End If
                Case 88
                    'steep slope
                    CX = (-0.5 + MapObj(i).X / 160)
                    CY = (-0.5 + MapObj(i).Y / 160)
                    If (MapObj(i).Flag \ &H100000) Mod &H2 = 0 Then
                        'Left oblique
                        G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\7.PNG"),
                                            CSng((CX) * Zm), (H-1) * Zm-CSng(CY * Zm), Zm, Zm)
                        G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\7.PNG"),
                                            CSng((CX + MapObj(i).W-1) * Zm), (H-1) * Zm-CSng((CY + MapObj(i).H-1) * Zm), Zm, Zm)
                        'G.DrawString(GroundNode(CX + 1, CY + 1), Me.Font, Brushes.Black, CSng((CX) * Zm), (H-1) * Zm-CSng(CY * Zm))
                        'G.DrawString(GroundNode(CX + MapObj(i).W, CY + MapObj(i).H), Me.Font, Brushes.Black, CSng((CX + MapObj(i).W-1) * Zm ), (H-1) * Zm-CSng((CY + MapObj(i).H-1) * Zm))
                        For j = 1 To MapObj(i).W-2
                            G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\88.PNG"),
                                                CSng((CX + j) * Zm),
                                                (H-1) * Zm-CSng((CY + j) * Zm), Zm, Zm * 2)
                            'G.DrawString(GroundNode(CX + 1 + j, CY + 1 + j), Me.Font, Brushes.Black, CSng((CX + j) * Zm), (H-1) * Zm-CSng(( CY + j) * Zm))
                            'G.DrawString(GroundNode(CX + 1 + j, CY + j), Me.Font, Brushes.Black, CSng((CX + j) * Zm), (H-1) * Zm-CSng((CY- 1 + j) * Zm))

                        Next
                    Else
                        'Right diagonal
                        G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\7.PNG"),
                                            CSng(CX * Zm),
                                            (H-1) * Zm-CSng((CY + MapObj(i).H-1) * Zm), Zm, Zm)
                        G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\7.PNG"),
                                            CSng((CX + MapObj(i).W-1) * Zm),
                                            (H-1) * Zm-CSng(CY * Zm), Zm, Zm)
                        'G.DrawString(GroundNode(CX + MapObj(i).W, CY + 1), Me.Font, Brushes.Black, CSng((CX + MapObj(i).W-1) * Zm), (H- 1) * Zm-CSng(CY * Zm))
                        'G.DrawString(GroundNode(CX + 1, CY + MapObj(i).W), Me.Font, Brushes.Black, CSng((CX) * Zm), (H-1) * Zm-CSng((CY + MapObj(i).H-1) * Zm))

                        For j = 1 To MapObj(i).W-2
                            G.DrawImage(Image.FromFile(PT & "\img\" & LH.GameStyle.ToString & "\obj\88A.PNG"),
                                                CSng((CX + j) * Zm),
                                                (H-1) * Zm-CSng((CY-j-1 + MapObj(i).W) * Zm), Zm, Zm * 2)
                            'G.DrawString(GroundNode(CX + 1 + j, CY-j-1 + MapObj(i).W), Me.Font, Brushes.Black, CSng((CX + j) * Zm), (H-1 ) * Zm-CSng((CY-j-2 + MapObj(i).W) * Zm))
                            'G.DrawString(GroundNode(CX + 1 + j, CY-j + MapObj(i).W), Me.Font, Brushes.Black, CSng((CX + j) * Zm), (H-1) * Zm-CSng((CY-j-1 + MapObj(i).W) * Zm))
                        Next
                    End If
            End Select
        Next
    End Sub
    Public Function GetGrdBold(x As Integer, y As Integer) As Integer
        If GroundNode(x, y + 1)> 1 Then'U
            GetGrdBold = GroundNode(x, y + 1)
        ElseIf GroundNode(x, y-1)> 1 Then'D
            GetGrdBold = GroundNode(x, y-1)
        Else
            GetGrdBold = 0
        End If
    End Function
    Public Function CalC(x1 As Integer, y1 As Integer, x2 As Integer, y2 As Integer,
                         x3 As Integer, y3 As Integer, x4 As Integer, y4 As Integer) As Integer
        CalC = IIf(GroundNode(x1, y1) = 0, 0, 1000) + IIf(GroundNode(x2, y2) = 0, 0, 100) + IIf(GroundNode(x3, y3) = 0, 0, 10) + IIf(GroundNode(x4, y4) = 0, 0, 1)
    End Function
    Public Function GetCorCode(x As Integer, y As Integer) As Point
        Dim C As Integer

        Select Case GetGrdBold(x, y)
            Case 2'D steep top left
                C = CalC(x-1, y, x-1, y + 1, x, y + 1, x + 1, y + 1)
                Select Case C
                    Case 0
                        GetCorCode = New Point(5, 30)
                    Case 1, 100, 101
                        GetCorCode = New
 Point(10, 13)
                    Case 10
                        GetCorCode = New Point(15, 26)
                    Case 11, 111
                        GetCorCode = New Point(9, 27)
                    Case 110
                        GetCorCode = New Point(15, 27)
                    Case 1000, 1001, 1100, 1101
                        GetCorCode = New Point(5, 27)Case 1010
                        GetCorCode = New Point(13, 27)
                    Case 1011
                        GetCorCode = New Point(7, 27)
                    Case 1110
                        GetCorCode = New Point(11, 27)
                    Case 1111
                        GetCorCode = New Point(1, 26)
                End Select
            Case 3'D steep top right
                C = CalC(x-1, y + 1, x, y + 1, x + 1, y + 1, x + 1, y)
                Select Case C
                    Case 0, 10, 1000, 1010
                        GetCorCode = New Point(4, 30)
                    Case 1, 11, 1001, 1011
                        GetCorCode = New Point(4, 27)
                    Case 100, 110
                        GetCorCode = New Point(14, 27)
                    Case 101
                        GetCorCode = New Point(12, 27)
                    Case 111
                        GetCorCode = New Point(10, 27)
                    Case 1100, 1110
                        GetCorCode = New Point(8, 27)
                    Case 1101
                        GetCorCode = New Point(6, 27)
                    Case 1111
                        GetCorCode = New Point(0, 26)
                End Select
            Case 4'U steep bottom left
                C = CalC(x-1, y, x-1, y-1, x, y-1, x + 1, y-1)
                Select Case C
                    Case 0, 1, 100, 101
                        GetCorCode = New Point(5, 29)
                    Case 10, 110
                        GetCorCode = New Point(15, 26)
                    Case 11, 111
                        GetCorCode = New Point(9, 26)
                    Case 1000, 1001, 1100, 1101
                        GetCorCode = New Point(5, 26)
                    Case 1010
                        GetCorCode = New Point(13, 26)
                    Case 1011
                        GetCorCode = New Point(7, 26)
                    Case 1110
                        GetCorCode = New Point(11, 26)
                    Case 1111
                        GetCorCode = New Point(1, 25)
                End Select
            Case 5'U steep bottom right
                C = CalC(x-1, y-1, x, y-1, x + 1, y-1, x + 1, y)
                Select Case C
                    Case 0, 10, 1000, 1010
                        GetCorCode = New Point(4, 29)
                    Case 1, 11, 1001, 1011
                        GetCorCode = New Point(4, 26)
                    Case 100, 110
                        GetCorCode = New Point(14, 26)
                    Case 101
                        GetCorCode = New Point(12, 26)
                    Case 111
                        GetCorCode = New Point(10, 26)
                    Case 1100, 1110
                        GetCorCode = New Point(8, 26)
                    Case 1101
                        GetCorCode = New Point(6, 26)
                    Case 1111
                        GetCorCode = New Point(0, 25)
                End Select
            Case 12'D Slowly Big Upper Left
                C = CalC(x-1, y, x-1, y + 1, x, y + 1, x + 1, y + 1)
                Select Case C
                    Case 0, 1, 100, 101
                        GetCorCode = New Point(5, 30)
                    Case 10, 110
                        GetCorCode = New Point(15, 30)
                    Case 11, 111
                        GetCorCode = New Point(13, 31)
                    Case 1000, 1001, 1100, 1101
                        GetCorCode = New Point(8, 31)
                    Case 1010
                        GetCorCode = New Point(13, 30)
                    Case 1011
                        GetCorCode = New Point(11, 31)
                    Case 1110
                        GetCorCode = New Point(11, 30)
                    Case 1111
                        GetCorCode = New Point(2, 30)
                End Select
            Case 13'D Slowly Big Upper Right
                C = CalC(x-1, y + 1, x, y + 1, x + 1, y + 1, x + 1, y)
                Select Case C
                    Case 0, 10, 1000, 1010
                        GetCorCode = New Point(4, 30)
                    Case 1, 11, 1001, 1011
                        GetCorCode = New Point(7, 31)
                    Case 100, 110
                        GetCorCode = New Point(14, 30)
                    Case 101
                        GetCorCode = New Point(12, 30)
                    Case 111
                        GetCorCode = New Point(10, 30)
                    Case 1100, 1110
                        GetCorCode = New Point(12, 31)
                    Case 1101
                        GetCorCode = New Point(10, 31)
                    Case 1111
                        GetCorCode = New Point(1, 30)
                End Select
            Case 14'U slowly big bottom left
                C = CalC(x-1, y, x-1, y-1, x, y-1, x + 1, y-1)
                Select Case C
                    Case 0, 1, 100, 101
                        GetCorCode = New Point(5, 29)
                    Case 10, 110
                        GetCorCode = New Point(15, 29)
