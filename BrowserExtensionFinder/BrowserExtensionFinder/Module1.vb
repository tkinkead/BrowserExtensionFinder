﻿Imports System.IO
Imports Newtonsoft.Json.Linq

Module Module1
    Public ExemptionList As New System.Collections.Generic.List(Of String)
    Public outputType As String = "Console"

    Sub Main()
        ParseArguments()

        If File.Exists("exemption.chrome") Then
            LoadExemptionList()
        End If

        Dim di As New IO.DirectoryInfo("C:\Users")
        Dim Dirs() As IO.DirectoryInfo = di.GetDirectories()

        For Each directory As IO.DirectoryInfo In Dirs
            Dim firefoxpath As String = directory.FullName + "\AppData\Roaming\Mozilla\Firefox\Profiles"

            If System.IO.Directory.Exists(firefoxpath) Then
                OutputFirefoxData(firefoxpath)
            End If

            Dim chromepath As String = directory.FullName + "\AppData\Local\Google\Chrome\User Data\Default\Extensions"

            If System.IO.Directory.Exists(chromepath) Then
                OutputChromeData(chromepath)
            End If
        Next

        Console.ReadLine()
    End Sub

    Sub ParseArguments()
        '/type type (CSV) (optional)
        '/help help (optional)

        Dim typeArgument As String = "--type="
        Dim typeName As String = ""
        Dim helpArgument As String = "--help"

        For Each s As String In My.Application.CommandLineArgs
            If s.ToLower.StartsWith(helpArgument) Then
                WriteHelpText()
                Environment.Exit(0)
            ElseIf s.ToLower.StartsWith(typeArgument) Then
                outputType = "CSV"
            Else
                WriteHelpText()
                Environment.Exit(0)
            End If
        Next
    End Sub

    Sub OutputFirefoxData(path As String)
        Try
            Dim difirefox As New IO.DirectoryInfo(path)
            Dim firefoxdirs() As IO.DirectoryInfo = difirefox.GetDirectories()

            For Each directory As IO.DirectoryInfo In firefoxdirs
                If outputType = "Console" Then
                    Console.WriteLine("FIREFOX - " + directory.FullName)
                    Console.WriteLine("-----------------------------------------------------")
                End If

                If System.IO.Directory.Exists(directory.FullName) Then
                    Dim firefoxjson As String = File.ReadAllText(directory.FullName + "\addons.json")

                    Dim o As JObject = JObject.Parse(firefoxjson)
                    Dim results As List(Of JToken) = o.Children().ToList

                    For Each item As JProperty In results
                        item.CreateReader()

                        Dim strName As String
                        Dim strType As String
                        Dim strVersion As String
                        Select Case item.Name
                            Case "addons"
                                For Each subitem As JObject In item.Values
                                    strName = subitem("name")
                                    strType = subitem("type")
                                    strVersion = subitem("version")

                                    FirefoxWriteOut(strName, strType, strVersion, directory.FullName)
                                Next
                        End Select

                    Next
                End If
            Next

        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
    End Sub

    Sub OutputChromeData(path As String)
        Try
            Dim dichrome As New IO.DirectoryInfo(path)
            Dim chromedirs() As IO.DirectoryInfo = dichrome.GetDirectories()

            If outputType = "Console" Then
                Console.WriteLine("CHROME - " + path)
                Console.WriteLine("-----------------------------------------------------")
            End If

            For Each directory As IO.DirectoryInfo In chromedirs
                If Not ExemptionList.Contains(directory.Name) Then
                    If outputType = "Console" Then
                        Console.WriteLine(directory.FullName)
                    End If

                    Dim subdir As New IO.DirectoryInfo(directory.FullName)
                    Dim subdirs() As IO.DirectoryInfo = subdir.GetDirectories()

                    For Each extdir As IO.DirectoryInfo In subdirs
                        'Parse extdir\manifest.json
                        Dim chromejson As String = File.ReadAllText(extdir.FullName + "\manifest.json")
                        Dim jss As Object = New System.Web.Script.Serialization.JavaScriptSerializer().Deserialize(Of Object)(chromejson)

                        ChromeWriteOut(jss("name"), jss("version"), jss("description"), extdir.FullName)

                    Next
                End If
            Next
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
    End Sub

    Public Class firefoxaddon
        Public Property listofitems() As firefoxitems()
    End Class

    Public Class firefoxitems
        Public Property name As String
        Public Property version As String
        Public Property type As String
        Public Property userdisabled As String
        Public Property iscompatible As String
        Public Property isblocklisted As String
    End Class

    Sub FirefoxWriteOut(name As String, type As String, version As String, path As String)
        If outputType = "CSV" Then
            'Browser, Path, Name, Verison, Description
            Console.WriteLine("'Firefox'," + path + "," + name + "," + version + "," + type)
        ElseIf outputType = "Console" Then
            Console.WriteLine("Name: " + name)
            Console.WriteLine("Type: " + type)
            Console.WriteLine("Version: " + version)
            Console.WriteLine("")
        End If
    End Sub

    Sub ChromeWriteOut(name As String, version As String, description As String, path As String)
        If outputType = "CSV" Then
            'Browser, Path, Name, Verison, Description
            Console.WriteLine("'Chrome'," + path + "," + name + "," + version + "," + description)
        ElseIf outputType = "Console" Then
            Console.WriteLine("Name: " + name)
            Console.WriteLine("Version: " + version)
            Console.WriteLine("Description: " + description)
            Console.WriteLine("")
        End If
    End Sub

    Sub LoadExemptionList()
        Using r As StreamReader = New StreamReader("exemption.chrome")
            Dim line As String

            line = r.ReadLine

            Do While (Not line Is Nothing)
                line = line.Trim()
                ExemptionList.Add(line)
                line = r.ReadLine
            Loop
        End Using
    End Sub

    Sub WriteHelpText()
        Console.WriteLine("Usage:")
        Console.WriteLine("--type=[type] -- Specify output type - options: Console (default), CSV")
        Console.WriteLine("--help -- This help prompt")
        Console.WriteLine("")
    End Sub
    'Dim dt As New DataTable

    ''Dim cnn As New SQLite.SQLiteConnection("Data Source=C:Users\Tanner\AppData\Roaming\Mozilla\Firefox\Profiles\9ztd0rkg.default\webappsstore.sqlite")
    'Dim cnn As New SQLite.SQLiteConnection("Data Source=" + directory.FullName + "\webappsstore.sqlite")
    'Dim cmd As New SQLite.SQLiteCommand(cnn)
    'cmd.CommandText = "SELECT value from webappsstore2 where key='discopane-url'"

    'cnn.Open()

    'Dim reader As SQLite.SQLiteDataReader = cmd.ExecuteReader()

    'dt.Load(reader)
    'reader.Close()
    'cnn.Close()

    'Dim firefoxjson As String = dt.Rows(0)(0).ToString()
    'firefoxjson = firefoxjson.Replace("%22", """")
    'firefoxjson = firefoxjson.Replace("%20", " ")
    'firefoxjson = firefoxjson.Split("#")(1)

    'Dim jss As Object = New System.Web.Script.Serialization.JavaScriptSerializer().Deserialize(Of Object)(firefoxjson)
End Module
