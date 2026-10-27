# Code Review Issues

ผลรีวิวโค้ดทั้ง repo (อ่านโค้ดอย่างเดียว ยังไม่ได้ build หรือรันแอป)
Verdict: **Request Changes**

## 🔴 Critical

- [ ] **C1 ย้ายไฟล์เกมของผู้ใช้โดยไม่ถาม** — [InitializeStore.xaml.cs:184-194](../PSPlibrary/InitializeStore.xaml.cs#L184-L194)
  ตอน Initialize ทุก `.iso/.cso/.pbp` ใน root ถูก `File.Move` เข้า subfolder ชื่อเดียวกัน ไม่มีการยืนยัน ถ้าชื่อซ้ำจะ throw กลางทางและเหลือสถานะค้าง
  แก้: ถามยืนยันก่อน, ตรวจชื่อซ้ำ, หรือเลิกย้ายไฟล์แล้ว scan แบบ recursive แทน
- [ ] **C2 `DeleteAllDirectory` ลบ cache จาก catch กว้าง** — [MainWindow.xaml.cs:383-390, 474-490](../PSPlibrary/MainWindow.xaml.cs#L474-L490)
  `catch {}` ใน `DataGame_SelectionChanged` ดักทุก exception (เช่น `Int64.Parse(Release)` ล้ม) แล้วเสนอลบ `PSPStore` ทั้งหมด และลบไม่ recurse
  แก้: catch เฉพาะ exception ที่คาดไว้, log สาเหตุจริง, ใช้ `Directory.Delete(path, true)`
- [ ] **C3 SFO parser ไม่ตรวจขอบเขต** — [SfoFile.cs:63-91](../PSPlibrary/SfoFile.cs#L63-L91)
  `_IndexTableEntries_` และ offset มาจากไฟล์โดยตรง ISO เสีย/จงใจทำมาทำให้ OutOfMemory หรืออ่านนอก array, `ConvertToKey` วนจน `0x00` โดยไม่เช็คความยาว, exception ถูกกลืนแล้วได้ object ที่โหลดครึ่งเดียว
  แก้: validate header/count/offset เทียบ `RawData.Length`, throw ให้ caller รู้
- [ ] **C4 SFO writer เสี่ยงทำ `setting.cfg` เสีย** — [SfoFile.cs:101-153](../PSPlibrary/SfoFile.cs#L101-L153)
  `ParamMaxLength = CountBytes + 1` ไม่มี padding/alignment, `UInt32.Parse(Value)` throw ถ้าไม่ใช่ตัวเลข (เช่น `"N/A"`) กลางการเขียน, `FileStream` ไม่มี `using` ไฟล์เสียแล้ว `MainWindow` ลบทิ้งสร้างใหม่ ค่าตั้งหาย
  แก้: เขียนลง temp แล้ว rename, ใช้ `using`, เช็คค่าก่อนเขียน
- [ ] **C5 Path traversal ผ่าน `DISC_ID`** — [InitializeStore.xaml.cs:89-99, 218](../PSPlibrary/InitializeStore.xaml.cs#L89-L99), [ViewerGame.xaml.cs:62-64](../PSPlibrary/ViewerGame.xaml.cs#L62-L64)
  `DISC_ID` จาก ISO ถูกต่อเป็น path โดยไม่ sanitize (`..\..\`) เขียนไฟล์นอก `PSPStore` ได้
  แก้: อนุญาตเฉพาะ `[A-Z0-9]` และตรวจว่า path ที่ resolve แล้วยังอยู่ใต้ `PSPStore`

## 🟠 High

- [ ] **H1 ลิงก์ donate ชี้โดเมนเก่า** — [MainWindow.xaml.cs:296](../PSPlibrary/MainWindow.xaml.cs#L296)
  `https://touno.io/donate` ไม่ตรงกับ `dvgamerr.app` ถ้าโดเมนหลุดมือจะพาผู้ใช้ไปเว็บคนอื่น
- [ ] **H2 `cronitor_token` ใน public repo** — [_config.yml:11](../_config.yml#L11)
  ถ้าเป็น token จริงให้ rotate และย้ายออก (ยังไม่ได้ตรวจว่า theme จำเป็นต้องใช้เป็น secret หรือไม่)

## 🟡 Suggestions

### Correctness / Error handling
- [ ] **S1** `PSPGame` ไม่ปิด `FileStream` เมื่อ throw `NotImplementedException` เป็น control flow — [PSPGame.cs:19-33](../PSPlibrary/PSPGame.cs#L19-L33) → ใช้ `using` และคืน `Readable=false` ตรง ๆ
- [ ] **S2** `ExtractFile`/`ReadAllByte` เปิด ISO ใหม่ทุกครั้ง (4-5 ครั้งต่อเกม), `Read` ครั้งเดียวอาจไม่ครบ, `CreateNew` throw ถ้ามีไฟล์แล้ว, เทียบ `ex.Message != "No such file"` เปราะบาง — [PSPGame.cs:36-90](../PSPlibrary/PSPGame.cs#L36-L90)
- [ ] **S3** `ReadAllByte` คืน `{0x0}` เมื่อล้มเหลว ทำให้ `DISC_ID` เป็น `"0"` และหลายเกมใช้ store folder เดียวกัน — [PSPGame.cs:68](../PSPlibrary/PSPGame.cs#L68)
- [ ] **S4** `catch` แล้วแค่ `CancelAsync()` ไม่แจ้งผู้ใช้/ไม่ log, `RunWorkerCompleted` throw `NotImplementedException` ทำให้แอป crash — [InitializeStore.xaml.cs:248-251, 264-267](../PSPlibrary/InitializeStore.xaml.cs#L248-L267)
- [ ] **S5** `GameLibrary` ขนาดเท่าจำนวนโฟลเดอร์ แต่ index เพิ่มต่อไฟล์ ISO → IndexOutOfRange ถ้าโฟลเดอร์เดียวมีหลาย ISO, ไม่มีโฟลเดอร์เลยได้ `-1` — [InitializeStore.xaml.cs:199-200](../PSPlibrary/InitializeStore.xaml.cs#L199-L200)
- [ ] **S6** `Item.GetType/GetRegion` ใช้ `Substring(0,1)`/`Substring(2,1)` throw เมื่อ id สั้นกว่า 3 ตัว — [Item.cs:15-37](../PSPlibrary/Item.cs#L15-L37)
- [ ] **S7** `ViewerGame`: `catch {}` รายงาน "Corrupt ISO" ทุกกรณี, `.pbp` เป็น case ว่าง, `e.Error != null` เป็น block ว่าง (หน้าจอค้าง), `Int64.Parse(Release)` throw ถ้าไม่ใช่ตัวเลข, `_ConfigTemp` ไม่ถูกใช้ — [ViewerGame.xaml.cs:62-91, 126](../PSPlibrary/ViewerGame.xaml.cs#L62-L126)
- [ ] **S8** `Double.Parse/Int32.Parse` ค่า config ไม่มี `TryParse`/`InvariantCulture` — locale ที่ใช้ `,` ทำให้ constructor throw แอปเปิดไม่ขึ้น — [MainWindow.xaml.cs:61-77](../PSPlibrary/MainWindow.xaml.cs#L61-L77) (ควรเลิกยืมรูปแบบ SFO มาเก็บ config)
- [ ] **S9** `Closing` โหลด `setting.cfg` ใหม่โดยไม่ try/catch ไฟล์หายแล้วปิดแอปไม่ได้ — [MainWindow.xaml.cs:243](../PSPlibrary/MainWindow.xaml.cs#L243)
- [ ] **S10** `Param(key)` คืน `"0"` เมื่อไม่พบ แยกจากค่า `0` จริงไม่ได้ (เหตุที่ UI มี `== "0"` กระจายทั่ว) — [SfoFile.cs:35-40](../PSPlibrary/SfoFile.cs#L35-L40) → ใช้ `Dictionary` แบบ `OrdinalIgnoreCase` และคืน null
- [ ] **S11** `ConvertToString` ตัด multi-byte ผิดสำหรับ lead byte `0xD0-0xDF`, `0xF0+` → ใช้ `Encoding.UTF8.GetString(data, index, length).TrimEnd('\0')` — [SfoFile.cs:204-271](../PSPlibrary/SfoFile.cs#L204-L271)

### Concurrency / Performance
- [ ] **S12** `_PreloadTitle/_PreloadSubject` เขียนจาก worker อ่านจาก UI ไม่มี sync → ส่งผ่าน `ReportProgress(percent, userState)` — [InitializeStore.xaml.cs:214-217](../PSPlibrary/InitializeStore.xaml.cs#L214-L217)
- [ ] **S13** `BeginInvoke` ไม่เรียก `EndInvoke`, `MessageBox.Show` จาก thread pool, `(DateTime)EditRelease.SelectedDate` throw ถ้าไม่ได้เลือกวัน → ใช้ `Task.Run`/`BackgroundWorker` — [MainWindow.xaml.cs:323-358, 499-524](../PSPlibrary/MainWindow.xaml.cs#L323-L358)
- [ ] **S14** `GetImage`/`BitmapFrame.Create` สร้างใหม่ทุกคลิก และ lock ไฟล์รูป ทำให้ `DeleteAllDirectory` ลบไม่ได้ → cache + `BitmapCacheOption.OnLoad` — [MainWindow.xaml.cs:128-131, 431-438](../PSPlibrary/MainWindow.xaml.cs#L128-L131)

### Maintainability
- [ ] **S15** handler `ImageSnap1..6`, `BorderSS1..6`, `SSImage1..6` ซ้ำ 6 เท่า และ `(Item)DataGame.SelectedValue` ไม่เช็ค null → handler เดียวอ่านจาก `sender`/`Tag`, เปลี่ยน `Snapshot1..6` เป็น `List<String>`
- [ ] **S16** `BinaryReader.cs` ไม่มีใครใช้ ชื่อชนกับ `System.IO.BinaryReader` และซ้ำกับ region "Convert Binary Method" ใน `SfoFile` → ลบทิ้ง
- [ ] **S17** สะกดผิด (`Catagory`, `Laguage`, `Maxinium`, `Cann't`), `_extenions = _extenions` ซ้ำ ([App.xaml.cs:19](../PSPlibrary/App.xaml.cs#L19)), โค้ด comment ทิ้งและ `btnMode_Click` ว่าง
- [ ] **S18** ไม่มี test เลย → เริ่มที่ `SfoFile` (Load/SaveAs round-trip, ไฟล์เสีย, multi-byte)

### Config / Supply chain
- [ ] **S19** target .NET 4.0 แต่ `packages.config` เป็น `net45`, DiscUtils 0.11 เก่ามาก, Debug|x86 ตั้ง `PlatformTarget=x64`; `.vs/…/.suo` และ `PSPlibrary.csproj.user` ถูก commit ทั้งที่ `.gitignore` มี `.vs/` → `git rm --cached` — [PSPlibrary.csproj:13](../PSPlibrary/PSPlibrary.csproj#L13)
- [ ] **S20** GitHub Actions ใช้ tag (`@v4`, `@v1`) ไม่ pin SHA — [gh-pages.yml:26-37](../.github/workflows/gh-pages.yml#L26-L37)

## ลำดับที่แนะนำ

1. C1, C2, C5 (ป้องกันข้อมูลผู้ใช้เสียหาย / เขียนนอกขอบเขต)
2. C3, C4 พร้อม test ของ `SfoFile` (S18)
3. H1, H2
4. S4, S5, S7 (ลด crash), ที่เหลือตามความสะดวก

## ข้อจำกัดของรีวิวนี้

- ไม่ได้ build หรือรันแอป ผลทั้งหมดมาจากการอ่านโค้ด
- ไฟล์รูป, `.suo`, `Resources.Designer.cs` และ XAML layout ไม่ได้ตรวจละเอียด
- `app.manifest` ยังไม่ได้ยืนยันว่า `requestedExecutionLevel` ที่ active คือ `asInvoker`
