﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xSkrape.APIWrapper;

namespace APIWrapperDemo
{
    internal static class InProcExamples
    {
        // Substitute your own client key, obtained by visiting https://www.xskrape.com, registering and adding a client query end-point
        // Credit usage is explained more here: https://www.xskrape.com/home/faq - usage is generally FREE up until a certain level of usage, or in the case of the in-process libraries (fixed charge per machine per month), your first month per machine can often be covered by free monthly credits as a "trial" period
        // Some examples below require your own client keys, etc. and may not return successful results without these being substituted in the code below (we have tested all examples with our own keys and got passes)
        // Something on your wish list? Have your own unique use cases? Contact me, community@codexframework.com
        // Other useful links:
        //    http://www.codexframework.com/Documentation/XSRefGuide?ProdFilter=xsapi 
        //    https://www.xskrape.com/Home/XSPageExplorer
        //    https://www.xskrape.com/Home/XSSQL

        const string CLIENT_KEY = "OBTAIN_KEY_FROM_XSKRAPE_DOT_COM";

        /// <summary>
        /// Invoke from a console app, for example, to run all examples.
        /// </summary>
        public static void RunAll()
        {
            Example1_TabularFromCSVHttp().Wait();
            Example2_TabularMultiPagedHTMLGrid().Wait();
            Example3_ClientSideAggregationOverTabularResults().Wait();
            Example4_MergeRequestFromWebAPIReturningShapedJSON().Wait();
            Example5_SingleValueFromRSSFeed().Wait();
            Example6_MergeMultipleRSSRequestsExtractingByLineNumber().Wait();
            Example7_ExtractMultipleElementsFromSinglePage().Wait();
            Example8_IsolateTabularDataOfInterestUsingTableIdentificationExpression().Wait();
            Example9_ExtractAndFlattenJSONOnHtmlPage().Wait();
            Example10_ExtractDataFromAnExcelFileLocatedInAZipFileStoredOnDropBox().Wait();
            Example11_TabularDataFromGoogleDocsSpreadsheet().Wait();
            Example12_PositionalBasedTabularDataInsidePreElement().Wait();

            // The following are specific to in-proc version since uses local file access
            Example13_LocalExcelFileWithMultiplePossibleTables().Wait();
            Example14_Last2000EntriesFromW3SVCLogFilesInDirectory().Wait();
            Example15_Last3DaysFromW3SVCLogFilesInDirectory().Wait();
        }

        public static async Task Example1_TabularFromCSVHttp()
        {
            try
            {
                // Example - pull csv data over http into a DataTable with appropriately typed columns and a max row count
                var r = await xSkrapeInProc.GetDataTable(CLIENT_KEY, "https://www.quandl.com/api/v1/datasets/WIKI/AAPL.csv?sort_order=desc&collapse=daily&trim_start=2016-01-01");

                Console.WriteLine($"Success: {r.success}");
                Console.WriteLine($"Message: {r.message}");

                if (r.data != null)
                {
                    Console.WriteLine(r.data.RenderDataTable());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public static async Task Example2_TabularMultiPagedHTMLGrid()
        {
            try
            {
                // Example - pull tabular data in a multi-page HTML grid (all pages) into a DataTable with appropriately typed columns
                var r = await xSkrapeInProc.GetDataTable(CLIENT_KEY, @"{ url:""https://www.codexframework.com/xskrape/sampledata1?page={0}"", method:""step_sequential"", stop_on:""any"", arguments:[{start:1, step:1}] }", "columnname=Quantity");

                Console.WriteLine($"Success: {r.success}");
                Console.WriteLine($"Message: {r.message}");

                if (r.data != null)
                {
                    Console.WriteLine(r.data.RenderDataTable());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public static async Task Example3_ClientSideAggregationOverTabularResults()
        {
            try
            {
                // Example - perform client-side aggregation on the HTML grid data used in the previous example
                var r = await xSkrapeInProc.GetDataTable(CLIENT_KEY, @"{ url:""https://www.codexframework.com/xskrape/sampledata1?page={0}"", method:""step_sequential"", stop_on:""any"", arguments:[{start:1, step:1}] }", "columnname=Quantity");

                Console.WriteLine($"Success: {r.success}");
                Console.WriteLine($"Message: {r.message}");

                if (r.data != null)
                {
                    Console.WriteLine((from a in r.data.AsDynamic() select a.Quantity).Max());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

        }

        public static async Task Example4_MergeRequestFromWebAPIReturningShapedJSON()
        {
            try
            {
                var siteKey = "USE_YOUR_API_KEY_HERE";
                var cityList = "London,UK|San Fransicso,CA|Juno,AK|Miami,FL|New York,NY|Dallas,TX|Calgary,AB";
                var url = @"{ url: ""http://api.openweathermap.org/data/2.5/weather?q={0}&units=metric&appid=" + siteKey + @""", method:""step_sequential_parse_json"", rows_path:""/root"", columns:[{path:""main/temp"", name:""Temperature"", datatype:""Double""}, {path:""wind/speed"", name:""Wind""}], arguments:[{parm_list:""" + cityList + @""", parm_column_name:""City""}]}";

                // Example - merge multiple requests using a parameterized list of cities for a weather API, returning tabular data from shaped JSON
                var r = await xSkrapeInProc.GetDataTable(CLIENT_KEY, url);

                Console.WriteLine($"Success: {r.success}");
                Console.WriteLine($"Message: {r.message}");

                if (r.data != null)
                {
                    Console.WriteLine(r.data.RenderDataTable());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
        
        public static async Task Example5_SingleValueFromRSSFeed()
        {
            try
            {
                var stationNumber = "46042";
                var url = $"http://www.ndbc.noaa.gov/data/latest_obs/{stationNumber}.rss";

                // Example - extract a single data element from an RSS feed (using a simple declarative expression of what the text is preceded by, versus needing to express it using xpath, etc.)
                var r = await xSkrapeInProc.GetSingle(CLIENT_KEY, url, "followinginnertext=Wind Speed");

                Console.WriteLine($"Success: {r.success}");
                Console.WriteLine($"Message: {r.message}");
                Console.WriteLine($"Data: {r.data}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public static async Task Example6_MergeMultipleRSSRequestsExtractingByLineNumber()
        {
            try
            {
                var stationNumbers = "46042|46026|46013|46011";
                var url = @"{ url: ""http://www.ndbc.noaa.gov/data/latest_obs/{0}.rss"", method:""step_sequential_parse_xml"", split_lines:""innertext"", rows_path:""/lines"", columns:[{path:""line[1]/text()"", name:""Name""}, {path:""line[16]/text()"", name:""WindSpeed"", parse:""regex=(?<=Wind Speed:.+?)\\d+\\.?\\d*"", datatype:""Double""}, {path:""line[16]/text()"", name:""AirTempF"", parse:""regex=(?<=Air Temperature.+?)\\d+\\.?\\d*"", datatype:""Double""}], arguments:[{parm_list:""" + stationNumbers + @""", parm_column_name:""Station""}]}";

                // Example - iterate a parameterized URL, pulling multiple data elements into a tabular format using split_lines setting (same RSS feed as example 5); split_lines shows how you can work with flat files, or in this case because the XML document contains a CDATA section, we define an alternative way to get at specific lines of text for the wind speed and temperature.
                var r = await xSkrapeInProc.GetDataTable(CLIENT_KEY, url);

                Console.WriteLine($"Success: {r.success}");
                Console.WriteLine($"Message: {r.message}");

                if (r.data != null)
                {
                    Console.WriteLine(r.data.RenderDataTable());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public static async Task Example7_ExtractMultipleElementsFromSinglePage()
        {
            try
            {
                var url = @"http://www.ndbc.noaa.gov/data/latest_obs/46042.rss";
                Dictionary<string, string> queries = new Dictionary<string, string>()
                {
                    { "name", "firstelement=title" },
                    { "windspeed", @"numberfollowsnear=Wind\ Speed" },
                    { "winddir", @"followinginnertext=Wind\ Direction" },
                    { "pubdate", @"xpath=/rss[1]/channel[1]/pubDate[1]/text() && validate=datetime" }
                };

                // Example - pull multiple data elements from a single page using different expression types (same RSS feed as example 5)
                //  Note - we obtained the above expressions as suggestions made by Page Explorer found here: https://www.xskrape.com/Home/XSPageExplorer
                var r = await xSkrapeInProc.GetMultiple(CLIENT_KEY, url, queries);

                Console.WriteLine($"Success: {r.success}");
                Console.WriteLine($"Message: {r.message}");

                if (r.data?.Count > 0)
                {
                    foreach (var kvp in r.data)
                    {
                        Console.WriteLine($"{kvp.Key} - {kvp.Value}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public static async Task Example8_IsolateTabularDataOfInterestUsingTableIdentificationExpression()
        {
            try
            {
                // Example - pull tabular HTML data from a page that has multiple tabular sources (uses a table identification expression to pick the one of interest, post-processing expression to fill in blank values when repeating has been suppressed)
                var r = await xSkrapeInProc.GetDataTable(CLIENT_KEY, "https://www.xskrape.com/Home/Rates", "columnname=Service && copywhenblank=Category && excludecolumns=Minimum");

                Console.WriteLine($"Success: {r.success}");
                Console.WriteLine($"Message: {r.message}");

                if (r.data != null)
                {
                    Console.WriteLine(r.data.RenderDataTable());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public static async Task Example9_ExtractAndFlattenJSONOnHtmlPage()
        {
            try
            {
                // Example - tabular results from JSON embedded on an HTML page, rendered using a dynamic object instead of a DataTable
                var r = await xSkrapeInProc.GetDataTable(CLIENT_KEY, "http://www.cnn.com", "jsonroot=siblings");

                Console.WriteLine($"Success: {r.success}");
                Console.WriteLine($"Message: {r.message}");

                if (r.data != null)
                {
                    foreach (var item in r.data.AsDynamic())
                    {
                        Console.WriteLine(item.headline);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public static async Task Example10_ExtractDataFromAnExcelFileLocatedInAZipFileStoredOnDropBox()
        {
            try
            {
                // For reference, see https://www.dropbox.com/developers/documentation/http/documentation#files-download
                var accessToken = "YOU_ACCESS_TOKEN_GOES_HERE";       // TODO - supply your own key associated with your own DropBox app

                // Example - tabular results from an Excel file hosted on a DropBox share, inside of a .zip file!
                var r = await xSkrapeInProc.GetDataTable(CLIENT_KEY, @"{ url: ""https://content.dropboxapi.com/2/files/download"", method:""none"", action:""POST"", headers:[""Authorization: Bearer " + accessToken + @""", ""Dropbox-API-Arg: {\""path\"": \""/sample_data.zip\""}""] }");

                Console.WriteLine($"Success: {r.success}");
                Console.WriteLine($"Message: {r.message}");

                if (r.data != null)
                {
                    Console.WriteLine(r.data.RenderDataTable());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public static async Task Example11_TabularDataFromGoogleDocsSpreadsheet()
        {
            try
            {
                // Example - tabular results from a Google Sheets spreadsheet (identifying the desired subset of data via a column name, plus showing proper type inference for dates); the URL is simply the "sharing link" provided by Google
                var r = await xSkrapeInProc.GetDataTable(CLIENT_KEY, "https://docs.google.com/spreadsheets/d/1r_gYGu8nawdIk7wpUrbL1evCqE0eygC-TZwVD9ViS-o/edit?usp=sharing", "columnname=Name");

                Console.WriteLine($"Success: {r.success}");
                Console.WriteLine($"Message: {r.message}");

                if (r.data != null)
                {
                    Console.WriteLine(r.data.RenderDataTable());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public static async Task Example12_PositionalBasedTabularDataInsidePreElement()
        {
            try
            {
                // Example - tabular data is not in a HTML table but in a text matrix inside a <pre> element; extract 4 columns of data based on position and length after applying chained filter terms and supporting a non-standard date format
                var r = await xSkrapeInProc.GetDataTable(CLIENT_KEY, @"{ url:""https://www.glonass-iac.ru/en/CUSGLONASS/index.php"", prefilter:""xpath=//pre && alltextafter=slot"", 
                            split_lines:""crlf"", rows_path:""/lines/*[position()>2]"", stop_on:""anybadrow"",
                            columns:[{name:""GLONASS"", position:4, length:3, datatype:""Int32""}, {name:""Cosmos"", position:11, length:4, datatype:""Int32""}, {name:""PlaneSlot"", position:18, length:4}, {name:""OutageDate"", position:70, length:10, format:""dd.MM.yyyy"", datatype:""DateTime""}] }");

                Console.WriteLine($"Success: {r.success}");
                Console.WriteLine($"Message: {r.message}");

                if (r.data != null)
                {
                    Console.WriteLine(r.data.RenderDataTable());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public static async Task Example13_LocalExcelFileWithMultiplePossibleTables()
        {
            try
            {
                // Example - Extract data from an Excel spreadsheet on a unc share
                var r = await xSkrapeInProc.GetDataTable(CLIENT_KEY, @"file:\\myserver01\datashare\spendanal.xlsx", "columnname=Month && mergecolumndown=2 && removerowswithblank=Month");

                Console.WriteLine($"Success: {r.success}");
                Console.WriteLine($"Message: {r.message}");

                if (r.data != null)
                {
                    Console.WriteLine(r.data.RenderDataTable());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public static async Task Example14_Last2000EntriesFromW3SVCLogFilesInDirectory()
        {
            try
            {
                // Example - Merge data from the all available W3SVC files located in a specific directory; limit to the most recent 200 rows of data
                var r = await xSkrapeInProc.GetDataTable(CLIENT_KEY, @"{ method: ""enumerate_files"", file_path:""C:\\temp\\samplelog"", file_sort:1 }", "rowlimit=200");

                Console.WriteLine($"Success: {r.success}");
                Console.WriteLine($"Message: {r.message}");

                if (r.data != null)
                {
                    Console.WriteLine(r.data.RenderDataTable());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

        public static async Task Example15_Last3DaysFromW3SVCLogFilesInDirectory()
        {
            try
            {
                // Example - Merge data from the most recent 3 W3SVC files located in a specific directory; return their file names in a column called "filename"
                var r = await xSkrapeInProc.GetDataTable(CLIENT_KEY, @"{ method: ""enumerate_files"", file_path:""C:\\temp\\samplelog"", file_sort:1, filename_columns:[{column_name:""filename""}], max_pass:3, max_pass_ok:true }");

                Console.WriteLine($"Success: {r.success}");
                Console.WriteLine($"Message: {r.message}");

                if (r.data != null)
                {
                    Console.WriteLine(r.data.RenderDataTable());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
