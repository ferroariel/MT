using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Highlight;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Version = Lucene.Net.Util.Version;

//http://www.ifdefined.com/blog/post/2009/02/Full-Text-Search-in-ASPNET-using-LuceneNET.aspx

namespace MT.Models
{
    public static class LuceneSearch
    {
        #region "metodosprivados"

        private static FSDirectory _directoryTemp;

        // booleano global usado para chequear si no hay ya un indexado en curso
        //public static bool indexado_en_curso = false;
        private static string _luceneDir = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "P/L");
        private static FSDirectory _directory
        {
            get
            {
                if (_directoryTemp == null) _directoryTemp = FSDirectory.Open(new DirectoryInfo(_luceneDir));
                if (IndexWriter.IsLocked(_directoryTemp)) IndexWriter.Unlock(_directoryTemp);
                var lockFilePath = Path.Combine(_luceneDir, "write.lock");
                if (File.Exists(lockFilePath)) File.Delete(lockFilePath);
                return _directoryTemp;
            }
        }

        private static void _addToLuceneIndex(MT_EMPRESA_PERFIL datos, IndexWriter writer)
        {
            var searchQuery = new TermQuery(new Term("NRO_EMPRESA", datos.NRO_EMPRESA.ToString()));
            writer.DeleteDocuments(searchQuery);
            var doc = new Document();
            doc.Add(new Field("NRO_EMPRESA", datos.NRO_EMPRESA.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("NRO_PAIS", datos.NRO_PAIS.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("NRO_PROVINCIA", datos.NRO_PROVINCIA.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("XDE_RAZONSOCIAL", datos.XDE_RAZONSOCIAL, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("XDE_CORTA", datos.XDE_CORTA + "", Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("OBS_MEDIA", datos.OBS_MEDIA + "", Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("MEM_LARGA", Varias.StripHTML(datos.MEM_LARGA) + "", Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("XDE_CP", datos.XDE_CP + "", Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("XDE_DOMICILIO", datos.XDE_DOMICILIO + "", Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("XDE_CIUDAD", datos.XDE_CIUDAD + "", Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("XDE_LAT", datos.XDE_LAT + "", Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("XDE_LON", datos.XDE_LON + "", Field.Store.YES, Field.Index.NOT_ANALYZED));
            MTrepository mtdb = new MTrepository();
            int[] rubs = mtdb.TagsdeEmpresa(datos.NRO_EMPRESA);
            foreach (int r in rubs)
            {
                doc.Add(new Field("NRO_RUBRO", r.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            }
            doc.Add(new Field("CAMPO_BUSQUEDA", String.Format("{0} {1} {2} {3}", datos.XDE_RAZONSOCIAL, datos.XDE_CORTA, Varias.StripHTML(datos.MEM_LARGA), datos.OBS_MEDIA), Field.Store.YES, Field.Index.ANALYZED));
            writer.AddDocument(doc);
        }

        private static void _addToLuceneIndex(MT_TAG datos, IndexWriter writer)
        {
            var searchQuery = new TermQuery(new Term("NRO_TAG", datos.NRO_RUBRO.ToString()));
            writer.DeleteDocuments(searchQuery);
            var doc = new Document();
            doc.Add(new Field("NRO_TAG", datos.NRO_RUBRO.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("NRO_TAG_PADRE", datos.NRO_PADRE.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("OBS_NOMBRE", datos.OBS_NOMBRE + "", Field.Store.YES, Field.Index.ANALYZED));
            writer.AddDocument(doc);
        }

        private static MT_EMPRESA_PERFIL _mapLuceneDocumentToData_E(Document doc)
        {
            return new MT_EMPRESA_PERFIL
            {
                NRO_EMPRESA = Convert.ToInt32(doc.Get("NRO_EMPRESA")),
                NRO_PAIS = Convert.ToInt32(doc.Get("NRO_PAIS")),
                NRO_PROVINCIA = Convert.ToInt32(doc.Get("NRO_PROVINCIA")),
                XDE_RAZONSOCIAL = doc.Get("XDE_RAZONSOCIAL"),
                XDE_CORTA = doc.Get("XDE_CORTA"),
                OBS_MEDIA = doc.Get("OBS_MEDIA"),
                MEM_LARGA = doc.Get("MEM_LARGA"),
                XDE_DOMICILIO = doc.Get("XDE_DOMICILIO"),
                XDE_CP = doc.Get("XDE_CP"),
                XDE_CIUDAD = doc.Get("XDE_CIUDAD"),
                XDE_LAT = doc.Get("XDE_LAT"),
                XDE_LON = doc.Get("XDE_LON")
            };
        }

        private static MT_TAG _mapLuceneDocumentToData_T(Document doc)
        {
            return new MT_TAG
            {
                NRO_RUBRO = Convert.ToInt32(doc.Get("NRO_TAG")),
                NRO_PADRE = Convert.ToInt32(doc.Get("NRO_TAG_PADRE")),
                OBS_NOMBRE = doc.Get("OBS_NOMBRE")
            };
        }

        private static IEnumerable<MT_EMPRESA_PERFIL> _mapLuceneToDataList_E(IEnumerable<Document> hits)
        {
            return hits.Select(_mapLuceneDocumentToData_E).ToList();
        }

        private static IEnumerable<MT_EMPRESA_PERFIL> _mapLuceneToDataList_E(IEnumerable<ScoreDoc> hits, IndexSearcher searcher)
        {
            return hits.Select(hit => _mapLuceneDocumentToData_E(searcher.Doc(hit.doc))).ToList();
        }

        private static IEnumerable<MT_TAG> _mapLuceneToDataList_T(IEnumerable<Document> hits)
        {
            return hits.Select(_mapLuceneDocumentToData_T).ToList();
        }

        private static IEnumerable<MT_TAG> _mapLuceneToDataList_T(IEnumerable<ScoreDoc> hits, IndexSearcher searcher)
        {
            return hits.Select(hit => _mapLuceneDocumentToData_T(searcher.Doc(hit.doc))).ToList();
        }

        private static void _removeDocFromLuceneIndex(MT_EMPRESA_PERFIL datos, IndexWriter writer)
        {
            var searchQuery = new TermQuery(new Term("NRO_EMPRESA", datos.NRO_EMPRESA.ToString()));
            writer.DeleteDocuments(searchQuery);
        }

        private static void _removeDocFromLuceneIndex(MT_TAG datos, IndexWriter writer)
        {
            var searchQuery = new TermQuery(new Term("NRO_TAG", datos.NRO_RUBRO.ToString()));
            writer.DeleteDocuments(searchQuery);
        }
        private static List<MT_EMPRESA_PERFIL> _search_E(Busqueda busqueda, ref int resultsCount, string searchField = "")
        {
            List<MT_EMPRESA_PERFIL> list = new List<MT_EMPRESA_PERFIL>();
            if (busqueda.Texto.Length > 2)
            {
                int PageIndex = busqueda.PageCurrent;
                PageIndex--;
                int PageSize = busqueda.PageSize;
                string searchQuery = String.Format("{0}*", busqueda.Texto);
                if (busqueda.Filtros.NRO_RUBRO != null)
                {
                    if (busqueda.Filtros.NRO_RUBRO.Count() > 0)
                    {
                        foreach (int r in busqueda.Filtros.NRO_RUBRO)
                        {
                            searchQuery += String.Format(" +NRO_RUBRO:{0}", r);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    string[] fieldList;
                    fieldList = new string[] { "CAMPO_BUSQUEDA" };
                    List<BooleanClause.Occur> occurs = new List<BooleanClause.Occur>();
                    foreach (string field in fieldList)
                    {
                        occurs.Add(BooleanClause.Occur.SHOULD);
                    }
                    var searcher = new IndexSearcher(_directory, false);
                    var analyzer = new StandardAnalyzer(Version.LUCENE_29);
                    Query qry = MultiFieldQueryParser.Parse(Version.LUCENE_29, searchQuery, fieldList, occurs.ToArray(), analyzer);
                    TopDocs topDocs = searcher.Search(qry, null, ((PageIndex + 1) * PageSize), Sort.RELEVANCE);
                    ScoreDoc[] scoreDocs = topDocs.ScoreDocs;
                    resultsCount = topDocs.TotalHits;
                    int StartRecPos = (PageIndex * PageSize) + 1;

                    SimpleHTMLFormatter formatter = new SimpleHTMLFormatter("<b>", "</b>");
                    SimpleFragmenter fragmenter = new SimpleFragmenter(50);
                    QueryScorer scorer = new QueryScorer(qry);
                    Highlighter highlighter = new Highlighter(formatter, scorer);
                    highlighter.SetTextFragmenter(fragmenter);

                    if (topDocs != null)
                    {
                        for (int i = (PageIndex * PageSize); i <= (((PageIndex + 1) * PageSize) - 1) && i < topDocs.ScoreDocs.Length; i++)
                        {
                            Document doc = searcher.Doc(topDocs.ScoreDocs[i].doc);

                            //string highlighted_text =
                            //highlighted_text = (highlighted_text == "") ? String.Empty : highlighted_text;
                            MT_EMPRESA_PERFIL p = _mapLuceneDocumentToData_E(doc);
                            TokenStream stream = analyzer.TokenStream("", new StringReader(doc.Get("CAMPO_BUSQUEDA")));
                            p.MEM_LARGA = highlighter.GetBestFragments(stream, doc.Get("CAMPO_BUSQUEDA"), 3, "...");
                            list.Add(p);
                        }
                    }
                }
            }
            return list;
        }

        private static List<MT_TAG> _search_T(Busqueda busqueda, ref int resultsCount, string searchField = "")
        {
            List<MT_TAG> list = new List<MT_TAG>();
            string searchQuery = busqueda.Texto/*String.Empty*/;

            //if .Format("+OBS_NOMBRE:{0}", busqueda.Texto);
            /*if (!string.IsNullOrEmpty(searchQuery))
            {   */
            string[] fieldList;
            fieldList = new string[] { "OBS_NOMBRE" };
            List<BooleanClause.Occur> occurs = new List<BooleanClause.Occur>();
            foreach (string field in fieldList)
            {
                occurs.Add(BooleanClause.Occur.SHOULD);
            }
            var searcher = new IndexSearcher(_directory, false);

            //string[] fieldList = { searchField };
            Query qry = MultiFieldQueryParser.Parse(Version.LUCENE_29, searchQuery, fieldList, occurs.ToArray(), new StandardAnalyzer(Version.LUCENE_29));
            TopDocs topDocs = searcher.Search(qry, null, ((/*PageIndex +*/ 1) /** PageSize*/), Sort.RELEVANCE);
            ScoreDoc[] scoreDocs = topDocs.ScoreDocs;
            resultsCount = topDocs.TotalHits;
            //int StartRecPos = /*(PageIndex * PageSize) + */ 1;
            if (topDocs != null)
            {
                for (int i = 0; i < topDocs.ScoreDocs.Length; i++)
                {
                    Document doc = searcher.Doc(topDocs.ScoreDocs[i].doc);
                    list.Add(_mapLuceneDocumentToData_T(doc));
                }
            }

            //}
            return list;
        }

        private static Query parseQuery(string searchQuery, QueryParser parser)
        {
            Query query;
            try
            {
                query = parser.Parse(searchQuery.Trim());
            }
            catch (ParseException)
            {
                query = parser.Parse(QueryParser.Escape(searchQuery.Trim()));
            }
            return query;
        }
        #endregion "metodosprivados"

        public static void AddUpdateLuceneIndex(IEnumerable<MT_EMPRESA_PERFIL> tags)
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_29);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var tag in tags) _addToLuceneIndex(tag, writer);
                analyzer.Close();
                writer.Close();
                writer.Dispose();
            }
        }

        public static void AddUpdateLuceneIndex(IEnumerable<MT_TAG> tags)
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_29);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                foreach (var tag in tags) _addToLuceneIndex(tag, writer);
                analyzer.Close();
                writer.Close();
                writer.Dispose();
            }
        }

        public static void AddUpdateLuceneIndex(MT_EMPRESA_PERFIL tag)
        {
            AddUpdateLuceneIndex(new List<MT_EMPRESA_PERFIL> { tag });
        }

        public static void AddUpdateLuceneIndex(MT_TAG tag)
        {
            AddUpdateLuceneIndex(new List<MT_TAG> { tag });
        }

        public static bool ClearLuceneIndex()
        {
            try
            {
                var analyzer = new StandardAnalyzer(Version.LUCENE_29);
                using (var writer = new IndexWriter(_directory, analyzer, true, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    writer.DeleteAll();
                    analyzer.Close();
                    writer.Close();
                    writer.Dispose();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static IEnumerable<MT_EMPRESA_PERFIL> GetAllIndexRecords()
        {
            if (!System.IO.Directory.EnumerateFiles(_luceneDir).Any()) return new List<MT_EMPRESA_PERFIL>();
            var searcher = new IndexSearcher(_directory, false);
            var reader = IndexReader.Open(_directory, false);
            var docs = new List<Document>();
            var term = reader.TermDocs();
            while (term.Next()) docs.Add(searcher.Doc(term.Doc()));
            reader.Close();
            reader.Dispose();
            searcher.Close();
            searcher.Dispose();
            return _mapLuceneToDataList_E(docs);
        }

        public static void Optimize()
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_29);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                analyzer.Close();
                writer.Optimize();
                writer.Close();
                writer.Dispose();
            }
        }

        public static void RemoveDocFromLuceneIndex(MT_EMPRESA_PERFIL ep)
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_29);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                _removeDocFromLuceneIndex(ep, writer);
                analyzer.Close();
                writer.Close();
                writer.Dispose();
            }
        }

        public static void RemoveDocFromLuceneIndex(MT_TAG ep)
        {
            var analyzer = new StandardAnalyzer(Version.LUCENE_29);
            using (var writer = new IndexWriter(_directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
            {
                _removeDocFromLuceneIndex(ep, writer);
                analyzer.Close();
                writer.Close();
                writer.Dispose();
            }
        }
        public static IEnumerable<MT_EMPRESA_PERFIL> Search_E(Busqueda busqueda, ref int contadorResultados, string fieldName = "")
        {
            string input = busqueda.Texto;
            if (string.IsNullOrEmpty(input)) return new List<MT_EMPRESA_PERFIL>();
            var terms = input.Trim().Replace("-", " ").Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);
            return _search_E(busqueda, ref contadorResultados, fieldName);
        }

        public static IEnumerable<MT_TAG> Search_T(Busqueda busqueda, ref int contadorResultados, string fieldName = "")
        {
            string input = busqueda.Texto;

            //if (string.IsNullOrEmpty(input)) return new List<MT_TAG>();
            var terms = input.Trim().Replace("-", " ").Split(' ')
                .Where(x => !string.IsNullOrEmpty(x)).Select(x => x.Trim() + "*");
            input = string.Join(" ", terms);
            return _search_T(busqueda, ref contadorResultados, fieldName);
        }

        public static IEnumerable<MT_EMPRESA_PERFIL> SearchDefault(Busqueda busqueda, string fieldName = "")
        {
            int g = 0;
            string input = busqueda.Texto;
            return string.IsNullOrEmpty(input) ? new List<MT_EMPRESA_PERFIL>() : _search_E(busqueda, ref g, fieldName);
        }
    }
}