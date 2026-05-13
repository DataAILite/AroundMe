Imports System
Imports System.Collections
Imports System.Collections.Specialized
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web
Imports System.Web.Caching
Imports System.Web.Script.Serialization
Imports System.Xml

Partial Class _Default
    Inherits System.Web.UI.Page

    Protected CurrentArea As String = "Phoenix, Arizona, United States"
    Protected PageTitleText As String = "AroundMe - Phoenix, Arizona"
    Private OsmDiagnostic As StringBuilder
    Private OsmLoadHadError As Boolean

    Private Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim area As String = Request.QueryString("area")
            If String.IsNullOrWhiteSpace(area) Then area = ""
            Dim loadPlaces As Boolean = String.Equals(Request.QueryString("loadplaces"), "1", StringComparison.OrdinalIgnoreCase)
            BindAroundMe(area.Trim(), loadPlaces, 0)
        End If
    End Sub

    Private Sub ButtonShow_Click(sender As Object, e As EventArgs) Handles ButtonShow.Click
        BindAroundMe(TextBoxLocation.Text.Trim())
    End Sub

    Private Sub ButtonLoadPlaces_Click(sender As Object, e As EventArgs) Handles ButtonLoadPlaces.Click
        Dim area As String = TextBoxLocation.Text.Trim()
        If String.IsNullOrWhiteSpace(area) Then area = CurrentArea
        Dim detailIndex As Integer = GetPostedDetailIndex()
        detailIndex = (detailIndex + 1) Mod 5
        BindAroundMe(area, True, detailIndex)
    End Sub

    Private Sub BindAroundMe(area As String, Optional loadPlaces As Boolean = False, Optional detailIndex As Integer = 0)
        detailIndex = NormalizeDetailIndex(detailIndex)
        If String.IsNullOrWhiteSpace(area) Then
            area = "Phoenix, Arizona, United States"
            TextBoxLocation.Text = ""
        Else
            TextBoxLocation.Text = area
        End If

        CurrentArea = area
        PageTitleText = "AroundMe - " & ShortAreaName(area)
        Page.Title = PageTitleText
        LabelTitle.Text = PageTitleText
        LabelSubtitle.Text = "Culture, tourism, food, events, and civic information around " & area & "."
        LabelNote.Text = "The first location is estimated from the browser IP address when available. You can type another place and refresh the dashboard."
        LiteralTiles.Text = BuildTiles(area)
        HiddenDetailIndex.Value = detailIndex.ToString(CultureInfo.InvariantCulture)
        LiteralRandomLinks.Text = BuildRandomLinks(area, loadPlaces, detailIndex)
        LabelOsmDiagnostic.Text = ""
        If loadPlaces AndAlso OsmLoadHadError AndAlso OsmDiagnostic IsNot Nothing Then
            LabelOsmDiagnostic.Text = Server.HtmlEncode(OsmDiagnostic.ToString())
        End If
    End Sub

    Private Function GetPostedDetailIndex() As Integer
        Dim detailIndex As Integer
        If Integer.TryParse(HiddenDetailIndex.Value, detailIndex) Then
            Return NormalizeDetailIndex(detailIndex)
        End If
        Return 0
    End Function

    Private Function NormalizeDetailIndex(detailIndex As Integer) As Integer
        If detailIndex < 0 Then Return 0
        Return detailIndex Mod 5
    End Function

    Private Function ShortAreaName(area As String) As String
        If String.IsNullOrWhiteSpace(area) Then Return "Nearby"
        Dim parts() As String = area.Split(","c)
        If parts.Length >= 2 Then Return parts(0).Trim() & ", " & parts(1).Trim()
        Return area.Trim()
    End Function

    Private Function GetTiles(area As String) As List(Of AroundMeTile)
        Dim tiles As New List(Of AroundMeTile)()
        tiles.Add(New AroundMeTile("Culture", "Arts, museums, public art, culture programs, historic places, and creative spaces near " & area & ".", "Culture search", "culture arts museums public art", "tileCulture", "https://images.unsplash.com/photo-1518998053901-5348d3961a04?auto=format&fit=crop&w=800&q=80",
            New List(Of AroundMeSuggestion) From {
                New AroundMeSuggestion("Museums", "best museums", "https://images.unsplash.com/photo-1566127992631-137a642a90f4?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Art galleries", "best art galleries", "https://images.unsplash.com/photo-1545987796-200677ee1011?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Historic places", "historic places landmarks", "https://images.unsplash.com/photo-1524230572899-a752b3835840?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Public art", "public art murals sculptures", "https://images.unsplash.com/photo-1579783902614-a3fb3927b6a5?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Live music", "live music venues", "https://images.unsplash.com/photo-1501612780327-45045538702b?auto=format&fit=crop&w=120&q=80")
            }))
        tiles.Add(New AroundMeTile("Tourism", "Visitor ideas, outdoor attractions, parks, trails, city highlights, and trip-planning information for " & area & ".", "Tourism search", "tourism attractions visitor guide parks trails", "tileTourism", "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=800&q=80",
            New List(Of AroundMeSuggestion) From {
                New AroundMeSuggestion("Top attractions", "top attractions", "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Parks", "best parks", "https://images.unsplash.com/photo-1441974231531-c6227db76b6e?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Walking tours", "walking tours", "https://images.unsplash.com/photo-1501555088652-021faa106b9b?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Visitor center", "visitor center guide", "https://images.unsplash.com/photo-1500534314209-a25ddb2bd429?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Day trips", "best day trips", "https://images.unsplash.com/photo-1507525428034-b723cf961d3e?auto=format&fit=crop&w=120&q=80")
            }))
        tiles.Add(New AroundMeTile("Eating", "Restaurants, local dining neighborhoods, food guides, cafes, and nearby places to eat around " & area & ".", "Dining search", "restaurants dining food cafes", "tileFood", "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?auto=format&fit=crop&w=800&q=80",
            New List(Of AroundMeSuggestion) From {
                New AroundMeSuggestion("Best restaurants", "best restaurants", "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Local cafes", "local cafes coffee", "https://images.unsplash.com/photo-1495474472287-4d71bcdd2085?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Breakfast spots", "best breakfast brunch", "https://images.unsplash.com/photo-1484723091739-30a097e8f929?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Food trucks", "food trucks", "https://images.unsplash.com/photo-1565123409695-7b5ef63a2efb?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Desserts", "best desserts bakeries", "https://images.unsplash.com/photo-1488477181946-6428a0291777?auto=format&fit=crop&w=120&q=80")
            }))
        tiles.Add(New AroundMeTile("Events", "Current and upcoming local events, weekend activities, performances, markets, and community programs near " & area & ".", "Events search", "current events this weekend things to do", "tileEvents", "https://images.unsplash.com/photo-1501281668745-f7f57925c3b4?auto=format&fit=crop&w=800&q=80",
            New List(Of AroundMeSuggestion) From {
                New AroundMeSuggestion("This weekend", "events this weekend", "https://images.unsplash.com/photo-1501281668745-f7f57925c3b4?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Concerts", "concerts", "https://images.unsplash.com/photo-1514525253161-7a46d19cd819?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Theater", "theater performances", "https://images.unsplash.com/photo-1503095396549-807759245b35?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Family events", "family events kids", "https://images.unsplash.com/photo-1511632765486-a01980e01a18?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Free events", "free events", "https://images.unsplash.com/photo-1511795409834-ef04bbd61622?auto=format&fit=crop&w=120&q=80")
            }))
        tiles.Add(New AroundMeTile("Festivals", "Local festivals, seasonal celebrations, music festivals, street fairs, art walks, and annual community events near " & area & ".", "Festival search", "festivals seasonal celebrations street fairs art walks", "tileEvents", "https://images.unsplash.com/photo-1492684223066-81342ee5ff30?auto=format&fit=crop&w=800&q=80",
            New List(Of AroundMeSuggestion) From {
                New AroundMeSuggestion("Music festivals", "music festivals", "https://images.unsplash.com/photo-1492684223066-81342ee5ff30?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Street fairs", "street fairs", "https://images.unsplash.com/photo-1533174072545-7a4b6ad7a6c3?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Art walks", "art walks", "https://images.unsplash.com/photo-1531058020387-3be344556be6?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Food festivals", "food festivals", "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Seasonal events", "seasonal festivals", "https://images.unsplash.com/photo-1519671482749-fd09be7ccebf?auto=format&fit=crop&w=120&q=80")
            }))
        tiles.Add(New AroundMeTile("Farmers Markets", "Farmers markets, local produce, craft food vendors, weekend markets, and community market days around " & area & ".", "Farmers market search", "farmers markets local produce weekend markets", "tileFood", "https://images.unsplash.com/photo-1488459716781-31db52582fe9?auto=format&fit=crop&w=800&q=80",
            New List(Of AroundMeSuggestion) From {
                New AroundMeSuggestion("Farmers markets", "farmers markets", "https://images.unsplash.com/photo-1488459716781-31db52582fe9?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Local produce", "local produce market", "https://images.unsplash.com/photo-1534723452862-4c874018d66d?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Weekend markets", "weekend markets", "https://images.unsplash.com/photo-1506484381205-f7945653044d?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Craft vendors", "craft vendors market", "https://images.unsplash.com/photo-1513116476489-7635e79feb27?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Farm stands", "farm stands", "https://images.unsplash.com/photo-1500937386664-56d1dfef3854?auto=format&fit=crop&w=120&q=80")
            }))
        tiles.Add(New AroundMeTile("Politics and Civic", "City services, local government, public meetings, civic updates, elections, and official community information for " & area & ".", "Civic search", "city government public meetings elections civic information", "tileCivic", "https://images.unsplash.com/photo-1529107386315-e1a2ed48a620?auto=format&fit=crop&w=800&q=80",
            New List(Of AroundMeSuggestion) From {
                New AroundMeSuggestion("City services", "city services official", "https://images.unsplash.com/photo-1486406146926-c627a92ad1ab?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Public meetings", "public meetings city council", "https://images.unsplash.com/photo-1521737604893-d14cc237f11d?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Elections", "elections voting information", "https://images.unsplash.com/photo-1540910419892-4a36d2c3266c?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Community boards", "community boards civic", "https://images.unsplash.com/photo-1556761175-b413da4baf72?auto=format&fit=crop&w=120&q=80"),
                New AroundMeSuggestion("Local news", "local news government", "https://images.unsplash.com/photo-1495020689067-958852a7765e?auto=format&fit=crop&w=120&q=80")
            }))
        Return tiles
    End Function

    Private Function BuildTiles(area As String) As String
        Dim sb As New StringBuilder()
        For Each tile As AroundMeTile In GetTiles(area)
            Dim tileUrl As String = "https://www.google.com/search?q=" & HttpUtility.UrlEncode(area & " " & tile.SearchText)
            sb.Append("<a class=""aroundTile ").Append(tile.CssClass).Append(""" href=""").Append(HttpUtility.HtmlAttributeEncode(tileUrl)).Append(""" target=""_blank"" data-topic=""").Append(HttpUtility.HtmlAttributeEncode(tile.Title)).Append(""" data-search=""").Append(HttpUtility.HtmlAttributeEncode(tile.SearchText)).Append(""" title=""Open ").Append(HttpUtility.HtmlAttributeEncode(tile.Title)).Append(""">")
            sb.Append("<img class=""tileImage"" src=""").Append(HttpUtility.HtmlAttributeEncode(tile.ImageUrl)).Append(""" alt=""").Append(HttpUtility.HtmlAttributeEncode(tile.Title)).Append(""" />")
            sb.Append("<span class=""tileContent"">")
            sb.Append("<span class=""tileBody"">")
            sb.Append("<span class=""tileTitle"">").Append(HttpUtility.HtmlEncode(tile.Title)).Append("</span>")
            sb.Append("<span class=""tileText"">").Append(HttpUtility.HtmlEncode(tile.Description)).Append("</span>")
            sb.Append("<span class=""tileSource"">").Append(HttpUtility.HtmlEncode(tile.SourceText)).Append("</span>")
            sb.Append("</span>")
            sb.Append("<span class=""tileBottom"">")
            sb.Append(BuildTileSuggestions(area, tile))
            sb.Append("</span>")
            sb.Append("</span>")
            sb.Append("</a>")
        Next
        Return sb.ToString()
    End Function

    Private Function BuildTileSuggestions(area As String, tile As AroundMeTile) As String
        Dim sb As New StringBuilder()
        sb.Append("<span class=""tileSuggestions"">")
        For Each suggestion As AroundMeSuggestion In tile.Suggestions
            Dim suggestionUrl As String = "https://www.google.com/search?q=" & HttpUtility.UrlEncode(area & " " & suggestion.SearchText)
            sb.Append("<span class=""suggestionLink"" data-href=""").Append(HttpUtility.HtmlAttributeEncode(suggestionUrl)).Append(""" data-search=""").Append(HttpUtility.HtmlAttributeEncode(suggestion.SearchText)).Append(""" onclick=""return aroundMeOpenSuggestion(event, this);"" title=""Open ").Append(HttpUtility.HtmlAttributeEncode(suggestion.Title)).Append(""">")
            sb.Append("<img class=""suggestionImage"" src=""").Append(HttpUtility.HtmlAttributeEncode(suggestion.ImageUrl)).Append(""" alt=""").Append(HttpUtility.HtmlAttributeEncode(suggestion.Title)).Append(""" />")
            sb.Append("<span class=""suggestionText"">").Append(HttpUtility.HtmlEncode(suggestion.Title)).Append("</span>")
            sb.Append("</span>")
        Next
        sb.Append("</span>")
        Return sb.ToString()
    End Function

    Private Function BuildRandomLinks(area As String, loadPlaces As Boolean, detailIndex As Integer) As String
        If Not loadPlaces Then
            Return "<span id=""OsmNotLoadedFlag"" style=""display:none;""></span>"
        End If

        OsmDiagnostic = New StringBuilder()
        OsmDiagnostic.AppendLine("OSM load started for: " & area)
        OsmDiagnostic.AppendLine("Detail item number: " & (detailIndex + 1).ToString(CultureInfo.InvariantCulture))
        Dim results As List(Of AroundMeResult) = GetCombinedOsmResults(area, detailIndex)
        OsmDiagnostic.AppendLine("Total accepted places: " & results.Count.ToString())

        ShuffleResults(results)

        Dim sb As New StringBuilder()
        Dim random As New Random(Guid.NewGuid().GetHashCode())
        Dim cells As List(Of AroundMeCell) = BuildRandomCells(10, 8)
        Dim maxLinks As Integer = Math.Min(Math.Min(results.Count, cells.Count), 40)
        If maxLinks = 0 Then
            OsmLoadHadError = True
            Return "<span id=""OsmLoadedEmptyFlag"" style=""display:none;""></span>"
        End If

        For i As Integer = 0 To maxLinks - 1
            Dim result As AroundMeResult = results(i)
            Dim cell As AroundMeCell = cells(i)
            Dim widthPixels As Integer = random.Next(86, 132)
            Dim style As String = "left:" & cell.LeftPercent.ToString() & "%;top:" & cell.TopPercent.ToString() & "%;width:" & widthPixels.ToString() & "px;height:26px;"

            sb.Append("<a class=""randomLink"" style=""").Append(HttpUtility.HtmlAttributeEncode(style)).Append(""" href=""").Append(HttpUtility.HtmlAttributeEncode(result.Url)).Append(""" target=""_blank"" data-topic=""").Append(HttpUtility.HtmlAttributeEncode(result.Topic)).Append(""" title=""Open ").Append(HttpUtility.HtmlAttributeEncode(result.Title)).Append(""">")
            sb.Append("<img class=""randomImage"" src=""").Append(HttpUtility.HtmlAttributeEncode(result.ImageUrl)).Append(""" alt=""").Append(HttpUtility.HtmlAttributeEncode(result.Title)).Append(""" />")
            sb.Append("<span class=""randomText"">").Append(HttpUtility.HtmlEncode(result.Title)).Append("</span>")
            sb.Append("</a>")
        Next
        OsmDiagnostic.AppendLine("Rendered random links: " & maxLinks.ToString())
        Return sb.ToString()
    End Function

    Private Function BuildRandomCells(columns As Integer, rows As Integer) As List(Of AroundMeCell)
        Dim cells As New List(Of AroundMeCell)()
        Dim random As New Random(Guid.NewGuid().GetHashCode())
        For row As Integer = 0 To rows - 1
            For column As Integer = 0 To columns - 1
                Dim leftPercent As Integer = CInt(Math.Round((column + 0.12 + (random.NextDouble() * 0.24)) * (94.0 / columns)))
                Dim topPercent As Integer = CInt(Math.Round((row + 0.12 + (random.NextDouble() * 0.24)) * (94.0 / rows)))
                cells.Add(New AroundMeCell(leftPercent, topPercent))
            Next
        Next
        ShuffleCells(cells)
        Return cells
    End Function

    Private Function GetSearchResults(area As String, categoryTitle As String, suggestion As AroundMeSuggestion) As List(Of AroundMeResult)
        EnsureOutboundSecurity()
        Dim cacheKey As String = "AroundMeOsmResults:v2:" & area & ":" & suggestion.SearchText
        Dim cached As Object = HttpRuntime.Cache(cacheKey)
        If cached IsNot Nothing Then
            Dim cachedRows As List(Of String) = TryCast(cached, List(Of String))
            If cachedRows IsNot Nothing AndAlso cachedRows.Count > 0 Then
                If OsmDiagnostic IsNot Nothing Then OsmDiagnostic.AppendLine("Cache hit: " & suggestion.Title & " -> " & cachedRows.Count.ToString())
                Return ResultRowsToResults(cachedRows)
            End If
            HttpRuntime.Cache.Remove(cacheKey)
        End If

        Dim results As New List(Of AroundMeResult)()
        'EnsureOutboundSecurity()

        Try
            Dim location As AroundMeLocation = GetAreaLocation(area)
            If location IsNot Nothing Then
                Dim loaded As List(Of AroundMeResult) = GetOverpassResults(location, categoryTitle, suggestion)
                AddDistinctResults(results, loaded)
                If OsmDiagnostic IsNot Nothing Then OsmDiagnostic.AppendLine(suggestion.Title & " -> " & results.Count.ToString() & " accepted")
            Else
                If OsmDiagnostic IsNot Nothing Then OsmDiagnostic.AppendLine(suggestion.Title & " -> no location")
            End If
        Catch ex As Exception
            If OsmDiagnostic IsNot Nothing Then OsmDiagnostic.AppendLine(suggestion.Title & " ERROR: " & ex.GetType().Name & " - " & ex.Message)
            System.Diagnostics.Debug.WriteLine("AroundMe GetSearchResults error for [" & area & " / " & suggestion.SearchText & "]: " & ex.ToString())
            Try
                System.Diagnostics.EventLog.WriteEntry("Application", "AroundMe GetSearchResults error for [" & area & " / " & suggestion.SearchText & "]: " & ex.ToString(), System.Diagnostics.EventLogEntryType.Warning)
            Catch
            End Try
        End Try

        HttpRuntime.Cache.Insert(cacheKey, ResultsToResultRows(results), Nothing, DateTime.Now.AddHours(6), Cache.NoSlidingExpiration)
        Return results
    End Function

    Private Function GetCombinedOsmResults(area As String, detailIndex As Integer) As List(Of AroundMeResult)
        EnsureOutboundSecurity()
        detailIndex = NormalizeDetailIndex(detailIndex)
        Dim cacheKey As String = "AroundMeCombinedOsmResults:v10-detail-" & detailIndex.ToString(CultureInfo.InvariantCulture) & ":" & area
        Dim cached As Object = HttpRuntime.Cache(cacheKey)
        If cached IsNot Nothing Then
            Dim cachedRows As List(Of String) = TryCast(cached, List(Of String))
            If cachedRows IsNot Nothing AndAlso cachedRows.Count > 0 Then
                If OsmDiagnostic IsNot Nothing Then OsmDiagnostic.AppendLine("Combined cache hit -> " & cachedRows.Count.ToString())
                Return ResultRowsToResults(cachedRows)
            End If
            HttpRuntime.Cache.Remove(cacheKey)
        End If

        Dim results As New List(Of AroundMeResult)()
        Try
            Dim location As AroundMeLocation = GetAreaLocation(area)
            If location Is Nothing Then
                OsmLoadHadError = True
                If OsmDiagnostic IsNot Nothing Then OsmDiagnostic.AppendLine("No location found.")
                Return results
            End If

            Dim tiles As List(Of AroundMeTile) = GetTiles(area)
            results = GetCombinedOverpassResults(location, tiles, detailIndex, 5)
        Catch ex As Exception
            OsmLoadHadError = True
            If OsmDiagnostic IsNot Nothing Then OsmDiagnostic.AppendLine("Combined OSM ERROR: " & ex.GetType().Name & " - " & ex.Message)
        End Try

        HttpRuntime.Cache.Insert(cacheKey, ResultsToResultRows(results), Nothing, DateTime.Now.AddHours(6), Cache.NoSlidingExpiration)
        Return results
    End Function

    Private Sub EnsureOutboundSecurity()
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
    End Sub

    Private Function GetAreaLocation(area As String) As AroundMeLocation
        Dim cacheKey As String = "AroundMeLocation:v1:" & area
        Dim cached As Object = HttpRuntime.Cache(cacheKey)
        If cached IsNot Nothing Then
            Dim cachedValue As String = TryCast(cached, String)
            If Not String.IsNullOrWhiteSpace(cachedValue) Then
                Dim parts() As String = cachedValue.Split("|"c)
                If parts.Length = 2 Then
                    Dim lat As Double
                    Dim lon As Double
                    If Double.TryParse(parts(0), NumberStyles.Float, CultureInfo.InvariantCulture, lat) AndAlso Double.TryParse(parts(1), NumberStyles.Float, CultureInfo.InvariantCulture, lon) Then
                        Return New AroundMeLocation(lat, lon)
                    End If
                End If
            End If
            HttpRuntime.Cache.Remove(cacheKey)
        End If

        Dim searchUrl As String = "https://nominatim.openstreetmap.org/search?format=json&limit=1&q=" & HttpUtility.UrlEncode(area)

        Using client As New AroundMeWebClient()
            client.Headers(HttpRequestHeader.UserAgent) = "AroundMe/1.0 (OpenStreetMap POI lookup)"
            client.Headers(HttpRequestHeader.Accept) = "application/json"
            Dim json As String = client.DownloadString(searchUrl)
            If OsmDiagnostic IsNot Nothing Then OsmDiagnostic.AppendLine("Nominatim response length: " & json.Length.ToString())
            Dim serializer As New JavaScriptSerializer()
            Dim items As List(Of Object) = JsonArrayToList(serializer.DeserializeObject(json))
            If OsmDiagnostic IsNot Nothing Then OsmDiagnostic.AppendLine("Nominatim item count: " & If(items Is Nothing, 0, items.Count).ToString())
            If items IsNot Nothing AndAlso items.Count > 0 Then
                Dim first As Dictionary(Of String, Object) = TryCast(items(0), Dictionary(Of String, Object))
                If first IsNot Nothing AndAlso first.ContainsKey("lat") AndAlso first.ContainsKey("lon") Then
                    Dim lat As Double
                    Dim lon As Double
                    If Double.TryParse(Convert.ToString(first("lat"), CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture, lat) AndAlso Double.TryParse(Convert.ToString(first("lon"), CultureInfo.InvariantCulture), NumberStyles.Float, CultureInfo.InvariantCulture, lon) Then
                        Dim cachedValue As String = lat.ToString(CultureInfo.InvariantCulture) & "|" & lon.ToString(CultureInfo.InvariantCulture)
                        HttpRuntime.Cache.Insert(cacheKey, cachedValue, Nothing, DateTime.Now.AddDays(7), Cache.NoSlidingExpiration)
                        Return New AroundMeLocation(lat, lon)
                    End If
                End If
            End If
        End Using

        Return Nothing
    End Function

    Private Sub AddDistinctResults(results As List(Of AroundMeResult), newResults As List(Of AroundMeResult))
        Dim seen As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For Each result As AroundMeResult In results
            seen.Add(result.Url)
        Next

        For Each result As AroundMeResult In newResults
            Dim uri As Uri = Nothing
            If Not Uri.TryCreate(result.Url, UriKind.Absolute, uri) Then Continue For
            If uri.Scheme <> Uri.UriSchemeHttp AndAlso uri.Scheme <> Uri.UriSchemeHttps Then Continue For
            If seen.Contains(uri.AbsoluteUri) Then Continue For
            seen.Add(uri.AbsoluteUri)
            results.Add(New AroundMeResult(result.Title, uri.AbsoluteUri, result.ImageUrl, result.Topic))
        Next
    End Sub

    Private Function ResultsToResultRows(results As List(Of AroundMeResult)) As List(Of String)
        Dim rows As New List(Of String)()
        For Each result As AroundMeResult In results
            rows.Add(result.Title & vbTab & result.Url & vbTab & result.ImageUrl & vbTab & result.Topic)
        Next
        Return rows
    End Function

    Private Function ResultRowsToResults(rows As List(Of String)) As List(Of AroundMeResult)
        Dim results As New List(Of AroundMeResult)()
        For Each row As String In rows
            Dim parts() As String = row.Split(ControlChars.Tab)
            If parts.Length >= 3 Then
                Dim topic As String = ""
                If parts.Length >= 4 Then topic = parts(3)
                results.Add(New AroundMeResult(parts(0), parts(1), parts(2), topic))
            End If
        Next
        Return results
    End Function

    Private Function GetOverpassResults(location As AroundMeLocation, categoryTitle As String, suggestion As AroundMeSuggestion) As List(Of AroundMeResult)
        Dim results As New List(Of AroundMeResult)()
        Dim filters As List(Of String) = GetOverpassFilters(suggestion.Title)
        If filters.Count = 0 Then Return results

        Dim query As String = BuildOverpassQuery(location, filters)
        Dim searchUrl As String = "https://overpass-api.de/api/interpreter?data=" & HttpUtility.UrlEncode(query)

        Using client As New AroundMeWebClient()
            client.Headers(HttpRequestHeader.UserAgent) = "AroundMe/1.0 (OpenStreetMap Overpass POI lookup)"
            client.Headers(HttpRequestHeader.Accept) = "application/json"
            Dim json As String = client.DownloadString(searchUrl)
            If OsmDiagnostic IsNot Nothing Then OsmDiagnostic.AppendLine("Overpass " & suggestion.Title & " response length: " & json.Length.ToString())
            Dim serializer As New JavaScriptSerializer()
            serializer.MaxJsonLength = 1024 * 1024 * 4
            Dim root As Dictionary(Of String, Object) = TryCast(serializer.DeserializeObject(json), Dictionary(Of String, Object))
            If root Is Nothing OrElse Not root.ContainsKey("elements") Then Return results

            Dim elements As List(Of Object) = JsonArrayToList(root("elements"))
            If OsmDiagnostic IsNot Nothing Then OsmDiagnostic.AppendLine("Overpass " & suggestion.Title & " element count: " & If(elements Is Nothing, 0, elements.Count).ToString())
            If elements Is Nothing Then Return results

            For Each item As Object In elements
                If results.Count >= 3 Then Exit For
                Dim element As Dictionary(Of String, Object) = TryCast(item, Dictionary(Of String, Object))
                If element Is Nothing Then Continue For

                Dim tags As Dictionary(Of String, Object) = Nothing
                If element.ContainsKey("tags") Then tags = TryCast(element("tags"), Dictionary(Of String, Object))
                Dim name As String = GetOsmName(tags)
                If String.IsNullOrWhiteSpace(name) Then Continue For
                If Not element.ContainsKey("type") OrElse Not element.ContainsKey("id") Then Continue For

                Dim osmType As String = Convert.ToString(element("type"), CultureInfo.InvariantCulture)
                Dim osmId As String = Convert.ToString(element("id"), CultureInfo.InvariantCulture)
                Dim url As String = "https://www.openstreetmap.org/" & osmType & "/" & osmId
                results.Add(New AroundMeResult(name, url, suggestion.ImageUrl, categoryTitle))
            Next
        End Using

        Return results
    End Function

    Private Function GetCombinedOverpassResults(location As AroundMeLocation, tiles As List(Of AroundMeTile), detailIndex As Integer, perSpecLimit As Integer) As List(Of AroundMeResult)
        Dim results As New List(Of AroundMeResult)()
        Dim specs As New List(Of AroundMeOsmSpec)()
        Dim allFilters As New List(Of String)()
        Dim seenFilters As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

        For Each tile As AroundMeTile In tiles
            If tile.Suggestions Is Nothing OrElse tile.Suggestions.Count = 0 Then Continue For
            Dim suggestion As AroundMeSuggestion = tile.Suggestions(Math.Min(detailIndex, tile.Suggestions.Count - 1))
            Dim filters As List(Of String) = GetOverpassFilters(suggestion.Title)
            If filters.Count = 0 Then Continue For
            specs.Add(New AroundMeOsmSpec(tile.Title, suggestion.Title, suggestion.ImageUrl, filters))
            For Each filter As String In filters
                If Not seenFilters.Contains(filter) Then
                    seenFilters.Add(filter)
                    allFilters.Add(filter)
                End If
            Next
        Next

        If allFilters.Count = 0 Then
            For Each tile As AroundMeTile In tiles
                Dim filters As List(Of String) = GetTopicOverpassFilters(tile.Title)
                If filters.Count = 0 Then Continue For
                specs.Add(New AroundMeOsmSpec(tile.Title, tile.Title, tile.ImageUrl, filters))
                For Each filter As String In filters
                    If Not seenFilters.Contains(filter) Then
                        seenFilters.Add(filter)
                        allFilters.Add(filter)
                    End If
                Next
            Next
        End If

        If allFilters.Count = 0 Then Return results

        Dim query As String = BuildBroadOverpassQuery(location)
        If OsmDiagnostic IsNot Nothing Then OsmDiagnostic.AppendLine("Overpass query: " & query)
        Dim searchUrl As String = "https://overpass-api.de/api/interpreter?data=" & HttpUtility.UrlEncode(query)
        Dim elements As List(Of Object) = Nothing

        Using client As New AroundMeWebClient()
            client.Headers(HttpRequestHeader.UserAgent) = "AroundMe/1.0 (OpenStreetMap Overpass POI lookup)"
            client.Headers(HttpRequestHeader.Accept) = "application/json"
            Dim json As String = client.DownloadString(searchUrl)
            If OsmDiagnostic IsNot Nothing Then OsmDiagnostic.AppendLine("Combined Overpass response length: " & json.Length.ToString())
            Dim serializer As New JavaScriptSerializer()
            serializer.MaxJsonLength = 1024 * 1024 * 4
            Dim root As Dictionary(Of String, Object) = TryCast(serializer.DeserializeObject(json), Dictionary(Of String, Object))
            If root Is Nothing OrElse Not root.ContainsKey("elements") Then Return results
            elements = JsonArrayToList(root("elements"))
        End Using

        If OsmDiagnostic IsNot Nothing Then OsmDiagnostic.AppendLine("Combined Overpass element count: " & If(elements Is Nothing, 0, elements.Count).ToString())
        If elements Is Nothing Then Return results

        Dim usedUrls As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)
        For Each spec As AroundMeOsmSpec In specs
            Dim acceptedForSpec As Integer = 0
            For Each item As Object In elements
                Dim element As Dictionary(Of String, Object) = TryCast(item, Dictionary(Of String, Object))
                If element Is Nothing Then Continue For

                Dim tags As Dictionary(Of String, Object) = Nothing
                If element.ContainsKey("tags") Then tags = TryCast(element("tags"), Dictionary(Of String, Object))
                If Not TagsMatchSpec(tags, spec) Then Continue For

                Dim name As String = GetOsmName(tags)
                If String.IsNullOrWhiteSpace(name) Then Continue For
                If Not element.ContainsKey("type") OrElse Not element.ContainsKey("id") Then Continue For

                Dim osmType As String = Convert.ToString(element("type"), CultureInfo.InvariantCulture)
                Dim osmId As String = Convert.ToString(element("id"), CultureInfo.InvariantCulture)
                Dim url As String = "https://www.openstreetmap.org/" & osmType & "/" & osmId
                If usedUrls.Contains(url) Then Continue For
                usedUrls.Add(url)
                results.Add(New AroundMeResult(name, url, spec.ImageUrl, spec.Topic))
                acceptedForSpec += 1
                If OsmDiagnostic IsNot Nothing AndAlso acceptedForSpec <= perSpecLimit Then OsmDiagnostic.AppendLine(spec.DetailTitle & " -> " & name)
                If acceptedForSpec >= perSpecLimit Then Exit For
            Next
            If OsmDiagnostic IsNot Nothing Then OsmDiagnostic.AppendLine(spec.DetailTitle & " accepted count: " & acceptedForSpec.ToString())
        Next

        Return results
    End Function

    Private Function TagsMatchSpec(tags As Dictionary(Of String, Object), spec As AroundMeOsmSpec) As Boolean
        If tags Is Nothing Then Return False
        For Each filter As String In spec.Filters
            If TagsMatchFilter(tags, filter) Then Return True
        Next
        Return False
    End Function

    Private Function TagsMatchFilter(tags As Dictionary(Of String, Object), filter As String) As Boolean
        If tags Is Nothing Then Return False
        If filter.Contains("""tourism""=""museum""") Then Return TagEquals(tags, "tourism", "museum")
        If filter.Contains("""tourism""=""gallery""") Then Return TagEquals(tags, "tourism", "gallery")
        If filter.Contains("""historic""") Then Return tags.ContainsKey("historic")
        Dim regexMatch As Match = Regex.Match(filter, "\[""([^""]+)""~""([^""]+)""\]")
        If regexMatch.Success Then
            Return TagIn(tags, regexMatch.Groups(1).Value, regexMatch.Groups(2).Value.Split("|"c))
        End If
        Dim equalsMatch As Match = Regex.Match(filter, "\[""([^""]+)""=""([^""]+)""\]")
        If equalsMatch.Success Then
            Return TagEquals(tags, equalsMatch.Groups(1).Value, equalsMatch.Groups(2).Value)
        End If
        If filter.Contains("""tourism""=""artwork""") Then Return TagEquals(tags, "tourism", "artwork")
        If filter.Contains("""amenity""=""music_venue""") Then Return TagEquals(tags, "amenity", "music_venue")
        If filter.Contains("""amenity""=""nightclub""") Then Return TagEquals(tags, "amenity", "nightclub")
        If filter.Contains("""amenity""=""bar""") Then Return TagEquals(tags, "amenity", "bar")
        If filter.Contains("""tourism""~""museum|gallery|artwork|attraction""") Then Return TagIn(tags, "tourism", New String() {"museum", "gallery", "artwork", "attraction"})
        If filter.Contains("""amenity""~""arts_centre|theatre""") Then Return TagIn(tags, "amenity", New String() {"arts_centre", "theatre"})
        If filter.Contains("""tourism""~""attraction|viewpoint|zoo|theme_park|aquarium""") Then Return TagIn(tags, "tourism", New String() {"attraction", "viewpoint", "zoo", "theme_park", "aquarium"})
        If filter.Contains("""tourism""~""attraction|viewpoint|museum|gallery|zoo|aquarium""") Then Return TagIn(tags, "tourism", New String() {"attraction", "viewpoint", "museum", "gallery", "zoo", "aquarium"})
        If filter.Contains("""leisure""=""park""") Then Return TagEquals(tags, "leisure", "park")
        If filter.Contains("""leisure""~""park|garden""") Then Return TagIn(tags, "leisure", New String() {"park", "garden"})
        If filter.Contains("""tourism""=""picnic_site""") Then Return TagEquals(tags, "tourism", "picnic_site")
        If filter.Contains("""tourism""=""information""") Then Return TagEquals(tags, "tourism", "information")
        If filter.Contains("""information""=""guidepost""") Then Return TagEquals(tags, "information", "guidepost")
        If filter.Contains("""amenity""=""restaurant""") Then Return TagEquals(tags, "amenity", "restaurant")
        If filter.Contains("""amenity""=""cafe""") Then Return TagEquals(tags, "amenity", "cafe")
        If filter.Contains("""amenity""~""restaurant|cafe""") Then Return TagIn(tags, "amenity", New String() {"restaurant", "cafe"})
        If filter.Contains("""amenity""~""restaurant|cafe|fast_food|bar|pub""") Then Return TagIn(tags, "amenity", New String() {"restaurant", "cafe", "fast_food", "bar", "pub"})
        If filter.Contains("""amenity""=""fast_food""") Then Return TagEquals(tags, "amenity", "fast_food")
        If filter.Contains("""shop""~""bakery|confectionery|ice_cream""") Then Return TagIn(tags, "shop", New String() {"bakery", "confectionery", "ice_cream"})
        If filter.Contains("""amenity""=""ice_cream""") Then Return TagEquals(tags, "amenity", "ice_cream")
        If filter.Contains("""amenity""=""community_centre""") Then Return TagEquals(tags, "amenity", "community_centre")
        If filter.Contains("""amenity""=""theatre""") Then Return TagEquals(tags, "amenity", "theatre")
        If filter.Contains("""amenity""=""arts_centre""") Then Return TagEquals(tags, "amenity", "arts_centre")
        If filter.Contains("""amenity""~""theatre|arts_centre|events_venue|conference_centre|community_centre""") Then Return TagIn(tags, "amenity", New String() {"theatre", "arts_centre", "events_venue", "conference_centre", "community_centre"})
        If filter.Contains("""amenity""=""marketplace""") Then Return TagEquals(tags, "amenity", "marketplace")
        If filter.Contains("""amenity""~""marketplace|community_centre|events_venue""") Then Return TagIn(tags, "amenity", New String() {"marketplace", "community_centre", "events_venue"})
        If filter.Contains("""shop""~""art|craft|gallery""") Then Return TagIn(tags, "shop", New String() {"art", "craft", "gallery"})
        If filter.Contains("""shop""~""farm|greengrocer""") Then Return TagIn(tags, "shop", New String() {"farm", "greengrocer"})
        If filter.Contains("""shop""~""supermarket|greengrocer|farm""") Then Return TagIn(tags, "shop", New String() {"supermarket", "greengrocer", "farm"})
        If filter.Contains("""amenity""~""townhall|post_office|library""") Then Return TagIn(tags, "amenity", New String() {"townhall", "post_office", "library"})
        If filter.Contains("""amenity""~""townhall|library|courthouse|police|post_office""") Then Return TagIn(tags, "amenity", New String() {"townhall", "library", "courthouse", "police", "post_office"})
        If filter.Contains("""amenity""~""townhall|courthouse|library""") Then Return TagIn(tags, "amenity", New String() {"townhall", "courthouse", "library"})
        If filter.Contains("""office""=""government""") Then Return TagEquals(tags, "office", "government")
        If filter.Contains("""amenity""~""townhall|community_centre""") Then Return TagIn(tags, "amenity", New String() {"townhall", "community_centre"})
        If filter.Contains("""office""=""newspaper""") Then Return TagEquals(tags, "office", "newspaper")
        Return False
    End Function

    Private Function TagEquals(tags As Dictionary(Of String, Object), key As String, value As String) As Boolean
        If Not tags.ContainsKey(key) Then Return False
        Return String.Equals(Convert.ToString(tags(key), CultureInfo.InvariantCulture), value, StringComparison.OrdinalIgnoreCase)
    End Function

    Private Function TagIn(tags As Dictionary(Of String, Object), key As String, values() As String) As Boolean
        If Not tags.ContainsKey(key) Then Return False
        Dim actual As String = Convert.ToString(tags(key), CultureInfo.InvariantCulture)
        For Each value As String In values
            If String.Equals(actual, value, StringComparison.OrdinalIgnoreCase) Then Return True
        Next
        Return False
    End Function

    Private Function BuildOverpassQuery(location As AroundMeLocation, filters As List(Of String)) As String
        Dim lat As String = location.Latitude.ToString(CultureInfo.InvariantCulture)
        Dim lon As String = location.Longitude.ToString(CultureInfo.InvariantCulture)
        Dim radius As String = "8000"
        Dim sb As New StringBuilder()
        sb.Append("[out:json][timeout:12];(")
        For Each filter As String In filters
            sb.Append("node(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")").Append(filter).Append(";")
            sb.Append("way(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")").Append(filter).Append(";")
            sb.Append("relation(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")").Append(filter).Append(";")
        Next
        sb.Append(");out center tags 100;")
        Return sb.ToString()
    End Function

    Private Function BuildBroadOverpassQuery(location As AroundMeLocation) As String
        Dim lat As String = location.Latitude.ToString(CultureInfo.InvariantCulture)
        Dim lon As String = location.Longitude.ToString(CultureInfo.InvariantCulture)
        Dim radius As String = "8000"
        Dim sb As New StringBuilder()
        sb.Append("[out:json][timeout:12];(")
        sb.Append("node(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")[name][amenity];")
        sb.Append("node(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")[name][tourism];")
        sb.Append("node(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")[name][leisure];")
        sb.Append("node(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")[name][shop];")
        sb.Append("node(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")[name][office];")
        sb.Append("node(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")[name][historic];")
        sb.Append("way(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")[name][amenity];")
        sb.Append("way(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")[name][tourism];")
        sb.Append("way(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")[name][leisure];")
        sb.Append("way(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")[name][shop];")
        sb.Append("way(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")[name][office];")
        sb.Append("way(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")[name][historic];")
        sb.Append("relation(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")[name][amenity];")
        sb.Append("relation(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")[name][tourism];")
        sb.Append("relation(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")[name][leisure];")
        sb.Append("relation(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")[name][shop];")
        sb.Append("relation(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")[name][office];")
        sb.Append("relation(around:").Append(radius).Append(",").Append(lat).Append(",").Append(lon).Append(")[name][historic];")
        sb.Append(");out center tags 1000;")
        Return sb.ToString()
    End Function

    Private Function GetOverpassFilters(suggestionTitle As String) As List(Of String)
        Select Case suggestionTitle
            Case "Museums"
                Return New List(Of String) From {"[""tourism""=""museum""]"}
            Case "Art galleries"
                Return New List(Of String) From {"[""tourism""=""gallery""]"}
            Case "Historic places"
                Return New List(Of String) From {"[""historic""]"}
            Case "Public art"
                Return New List(Of String) From {"[""tourism""=""artwork""]"}
            Case "Live music", "Concerts"
                Return New List(Of String) From {"[""amenity""=""music_venue""]", "[""amenity""=""nightclub""]", "[""amenity""=""bar""]", "[""amenity""=""pub""]", "[""amenity""=""theatre""]"}
            Case "Top attractions", "Day trips"
                Return New List(Of String) From {"[""tourism""~""attraction|viewpoint|zoo|theme_park|aquarium|museum|gallery""]", "[""historic""]", "[""leisure""~""park|garden""]"}
            Case "Parks"
                Return New List(Of String) From {"[""leisure""~""park|garden|nature_reserve|playground""]", "[""tourism""=""picnic_site""]"}
            Case "Walking tours", "Visitor center"
                Return New List(Of String) From {"[""tourism""=""information""]", "[""information""=""guidepost""]", "[""tourism""~""attraction|viewpoint|museum|gallery""]", "[""historic""]", "[""leisure""~""park|garden""]"}
            Case "Best restaurants"
                Return New List(Of String) From {"[""amenity""=""restaurant""]"}
            Case "Local cafes"
                Return New List(Of String) From {"[""amenity""=""cafe""]"}
            Case "Breakfast spots"
                Return New List(Of String) From {"[""amenity""~""restaurant|cafe""]"}
            Case "Food trucks"
                Return New List(Of String) From {"[""amenity""~""fast_food|restaurant|cafe|bar|pub""]"}
            Case "Desserts"
                Return New List(Of String) From {"[""shop""~""bakery|confectionery|ice_cream|supermarket|greengrocer""]", "[""amenity""~""ice_cream|cafe|restaurant""]"}
            Case "This weekend", "Family events", "Free events"
                Return New List(Of String) From {"[""amenity""~""community_centre|theatre|arts_centre|events_venue|conference_centre|library""]", "[""leisure""~""park|garden|playground""]", "[""tourism""~""attraction|museum|gallery""]"}
            Case "Theater"
                Return New List(Of String) From {"[""amenity""~""theatre|arts_centre|events_venue|conference_centre""]"}
            Case "Music festivals", "Street fairs", "Food festivals", "Seasonal events", "Weekend markets"
                Return New List(Of String) From {"[""amenity""~""marketplace|community_centre|events_venue|conference_centre|theatre|arts_centre""]", "[""leisure""~""park|garden""]", "[""tourism""~""attraction|museum|gallery""]", "[""shop""~""supermarket|greengrocer|farm|bakery|confectionery""]"}
            Case "Farmers markets"
                Return New List(Of String) From {"[""amenity""~""marketplace|community_centre|events_venue""]", "[""shop""~""supermarket|greengrocer|farm|bakery|confectionery|convenience""]", "[""leisure""~""park|garden""]"}
            Case "Art walks", "Craft vendors"
                Return New List(Of String) From {"[""shop""~""art|craft|gallery""]", "[""tourism""=""gallery""]"}
            Case "Local produce", "Farm stands"
                Return New List(Of String) From {"[""shop""~""farm|greengrocer|supermarket|convenience|bakery|confectionery""]", "[""amenity""~""marketplace|restaurant|cafe""]"}
            Case "City services"
                Return New List(Of String) From {"[""amenity""~""townhall|post_office|library""]", "[""office""=""government""]"}
            Case "Public meetings"
                Return New List(Of String) From {"[""amenity""~""townhall|community_centre|library|courthouse""]", "[""office""=""government""]"}
            Case "Elections"
                Return New List(Of String) From {"[""amenity""~""townhall|library|courthouse|community_centre""]", "[""office""=""government""]"}
            Case "Community boards"
                Return New List(Of String) From {"[""amenity""=""community_centre""]", "[""amenity""=""library""]"}
            Case "Local news"
                Return New List(Of String) From {"[""office""=""newspaper""]", "[""amenity""=""library""]"}
        End Select

        Return New List(Of String)()
    End Function

    Private Function GetTopicOverpassFilters(topicTitle As String) As List(Of String)
        Select Case topicTitle
            Case "Culture"
                Return New List(Of String) From {"[""tourism""~""museum|gallery|artwork|attraction""]", "[""historic""]", "[""amenity""~""arts_centre|theatre""]"}
            Case "Tourism"
                Return New List(Of String) From {"[""tourism""~""attraction|viewpoint|museum|gallery|zoo|aquarium""]", "[""leisure""~""park|garden""]"}
            Case "Eating"
                Return New List(Of String) From {"[""amenity""~""restaurant|cafe|fast_food|bar|pub""]"}
            Case "Events"
                Return New List(Of String) From {"[""amenity""~""theatre|arts_centre|events_venue|conference_centre|community_centre""]", "[""tourism""=""attraction""]"}
            Case "Festivals"
                Return New List(Of String) From {"[""amenity""~""marketplace|community_centre|events_venue""]", "[""leisure""=""park""]"}
            Case "Farmers Markets"
                Return New List(Of String) From {"[""amenity""=""marketplace""]", "[""shop""~""supermarket|greengrocer|farm""]"}
            Case "Politics and Civic"
                Return New List(Of String) From {"[""amenity""~""townhall|courthouse|library""]", "[""office""=""government""]"}
        End Select

        Return New List(Of String)()
    End Function

    Private Function GetOsmName(tags As Dictionary(Of String, Object)) As String
        If tags Is Nothing Then Return ""
        If tags.ContainsKey("name") Then Return Convert.ToString(tags("name"), CultureInfo.InvariantCulture)
        If tags.ContainsKey("operator") Then Return Convert.ToString(tags("operator"), CultureInfo.InvariantCulture)
        Return ""
    End Function

    Private Function JsonArrayToList(value As Object) As List(Of Object)
        If value Is Nothing Then Return Nothing

        Dim list As New List(Of Object)()
        Dim arrayList As ArrayList = TryCast(value, ArrayList)
        If arrayList IsNot Nothing Then
            For Each item As Object In arrayList
                list.Add(item)
            Next
            Return list
        End If

        Dim objectArray As Object() = TryCast(value, Object())
        If objectArray IsNot Nothing Then
            For Each item As Object In objectArray
                list.Add(item)
            Next
            Return list
        End If

        Return Nothing
    End Function

    Private Sub ShuffleResults(results As List(Of AroundMeResult))
        Dim random As New Random(Guid.NewGuid().GetHashCode())
        For i As Integer = results.Count - 1 To 1 Step -1
            Dim j As Integer = random.Next(i + 1)
            Dim current As AroundMeResult = results(i)
            results(i) = results(j)
            results(j) = current
        Next
    End Sub

    Private Sub ShuffleCells(cells As List(Of AroundMeCell))
        Dim random As New Random(Guid.NewGuid().GetHashCode())
        For i As Integer = cells.Count - 1 To 1 Step -1
            Dim j As Integer = random.Next(i + 1)
            Dim current As AroundMeCell = cells(i)
            cells(i) = cells(j)
            cells(j) = current
        Next
    End Sub

    Private Class AroundMeTile
        Public Property Title As String
        Public Property Description As String
        Public Property SourceText As String
        Public Property SearchText As String
        Public Property CssClass As String
        Public Property ImageUrl As String
        Public Property Suggestions As List(Of AroundMeSuggestion)

        Public Sub New(title As String, description As String, sourceText As String, searchText As String, cssClass As String, imageUrl As String, suggestions As List(Of AroundMeSuggestion))
            Me.Title = title
            Me.Description = description
            Me.SourceText = sourceText
            Me.SearchText = searchText
            Me.CssClass = cssClass
            Me.ImageUrl = imageUrl
            Me.Suggestions = suggestions
        End Sub
    End Class

    Private Class AroundMeSuggestion
        Public Property Title As String
        Public Property SearchText As String
        Public Property ImageUrl As String

        Public Sub New(title As String, searchText As String, imageUrl As String)
            Me.Title = title
            Me.SearchText = searchText
            Me.ImageUrl = imageUrl
        End Sub
    End Class

    Private Class AroundMeResult
        Public Property Title As String
        Public Property Url As String
        Public Property ImageUrl As String
        Public Property Topic As String

        Public Sub New(title As String, url As String, imageUrl As String, topic As String)
            Me.Title = title
            Me.Url = url
            Me.ImageUrl = imageUrl
            Me.Topic = topic
        End Sub
    End Class

    Private Class AroundMeLocation
        Public Property Latitude As Double
        Public Property Longitude As Double

        Public Sub New(latitude As Double, longitude As Double)
            Me.Latitude = latitude
            Me.Longitude = longitude
        End Sub
    End Class

    Private Class AroundMeOsmSpec
        Public Property Topic As String
        Public Property DetailTitle As String
        Public Property ImageUrl As String
        Public Property Filters As List(Of String)

        Public Sub New(topic As String, detailTitle As String, imageUrl As String, filters As List(Of String))
            Me.Topic = topic
            Me.DetailTitle = detailTitle
            Me.ImageUrl = imageUrl
            Me.Filters = filters
        End Sub
    End Class

    Private Class AroundMeCell
        Public Property LeftPercent As Integer
        Public Property TopPercent As Integer

        Public Sub New(leftPercent As Integer, topPercent As Integer)
            Me.LeftPercent = leftPercent
            Me.TopPercent = topPercent
        End Sub
    End Class

    Private Class AroundMeWebClient
        Inherits WebClient

        Protected Overrides Function GetWebRequest(address As Uri) As WebRequest
            Dim request As WebRequest = MyBase.GetWebRequest(address)
            request.Timeout = 30000
            Return request
        End Function
    End Class
End Class
