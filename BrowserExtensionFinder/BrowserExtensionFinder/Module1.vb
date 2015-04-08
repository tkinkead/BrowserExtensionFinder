Imports System.IO
Imports Newtonsoft.Json.Linq

Module Module1

    Sub Main()
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

    Sub OutputFirefoxData(path As String)
        Try
            Dim difirefox As New IO.DirectoryInfo(path)
            Dim firefoxdirs() As IO.DirectoryInfo = difirefox.GetDirectories()

            For Each directory As IO.DirectoryInfo In firefoxdirs
                Console.WriteLine("FIREFOX - " + directory.FullName)
                Console.WriteLine("-----------------------------------------------------")

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

                                    Console.WriteLine("Name: " + strName)
                                    Console.WriteLine("Type: " + strType)
                                    Console.WriteLine("Version: " + strVersion)
                                    Console.WriteLine("")
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
            Dim ExemptionList As New System.Collections.Generic.List(Of String)
            'Add exempted extensions here - REWRITE THIS TO ACCEPT EXTENSION FILE INPUT
            ExemptionList.Add("aapocclcgogkmnckokdopfmhonfmgoek")

            Dim dichrome As New IO.DirectoryInfo(path)
            Dim chromedirs() As IO.DirectoryInfo = dichrome.GetDirectories()

            Console.WriteLine("CHROME - " + path)
            Console.WriteLine("-----------------------------------------------------")

            For Each directory As IO.DirectoryInfo In chromedirs
                If Not ExemptionList.Contains(directory.Name) Then
                    Console.WriteLine(directory.FullName)

                    Dim subdir As New IO.DirectoryInfo(directory.FullName)
                    Dim subdirs() As IO.DirectoryInfo = subdir.GetDirectories()

                    For Each extdir As IO.DirectoryInfo In subdirs
                        'Parse extdir\manifest.json
                        Dim chromejson As String = File.ReadAllText(extdir.FullName + "\manifest.json")
                        Dim jss As Object = New System.Web.Script.Serialization.JavaScriptSerializer().Deserialize(Of Object)(chromejson)

                        Console.WriteLine("Name: " + jss("name"))
                        Console.WriteLine("Version: " + jss("version"))
                        Console.WriteLine("Description: " + jss("description"))
                        Console.WriteLine("")
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
