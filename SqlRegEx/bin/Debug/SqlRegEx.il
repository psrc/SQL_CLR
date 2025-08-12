
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.8.3928.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v2.0.50727
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 2:0:0:0
}
.assembly extern System.Data
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 2:0:0:0
}
.assembly extern System
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 2:0:0:0
}
.assembly SqlRegEx
{
  .custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = ( 01 00 20 43 6F 70 79 72 69 67 68 74 20 C2 A9 20   // .. Copyright .. 
                                                                                                  53 74 65 76 65 20 41 62 72 61 68 61 6D 20 20 32   // Steve Abraham  2
                                                                                                  30 30 37 00 00 )                                  // 007..
  .custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = ( 01 00 08 53 71 6C 52 65 67 45 78 00 00 )          // ...SqlRegEx..
  .custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = ( 01 00 08 53 71 6C 52 65 67 45 78 00 00 )          // ...SqlRegEx..
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = ( 01 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = ( 01 00 24 62 65 65 38 33 66 36 33 2D 35 32 30 66   // ..$bee83f63-520f
                                                                                                  2D 34 38 65 66 2D 38 63 63 61 2D 66 65 36 65 62   // -48ef-8cca-fe6eb
                                                                                                  65 63 61 32 37 34 34 00 00 )                      // eca2744..
  .custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = ( 01 00 07 31 2E 30 2E 30 2E 30 00 00 )             // ...1.0.0.0..
  .custom instance void [mscorlib]System.CLSCompliantAttribute::.ctor(bool) = ( 01 00 01 00 00 ) 
  .custom instance void [mscorlib]System.Resources.NeutralResourcesLanguageAttribute::.ctor(string) = ( 01 00 05 65 6E 2D 55 53 00 00 )                   // ...en-US..

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module SqlRegEx.dll
// MVID: {284A45F3-AF4D-4A7F-B5E0-96272ED259C4}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0C1D0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi sealed beforefieldinit SqlClrTools.SqlRegEx
       extends [mscorlib]System.Object
{
  .method public hidebysig static valuetype [System.Data]System.Data.SqlTypes.SqlBoolean 
          RegExIsMatch(valuetype [System.Data]System.Data.SqlTypes.SqlString input,
                       valuetype [System.Data]System.Data.SqlTypes.SqlString pattern,
                       valuetype [System.Data]System.Data.SqlTypes.SqlBoolean ignoreCase) cil managed
  {
    .custom instance void [System.Data]Microsoft.SqlServer.Server.SqlFunctionAttribute::.ctor() = ( 01 00 05 00 54 55 79 4D 69 63 72 6F 73 6F 66 74   // ....TUyMicrosoft
                                                                                                    2E 53 71 6C 53 65 72 76 65 72 2E 53 65 72 76 65   // .SqlServer.Serve
                                                                                                    72 2E 44 61 74 61 41 63 63 65 73 73 4B 69 6E 64   // r.DataAccessKind
                                                                                                    2C 20 53 79 73 74 65 6D 2E 44 61 74 61 2C 20 56   // , System.Data, V
                                                                                                    65 72 73 69 6F 6E 3D 32 2E 30 2E 30 2E 30 2C 20   // ersion=2.0.0.0, 
                                                                                                    43 75 6C 74 75 72 65 3D 6E 65 75 74 72 61 6C 2C   // Culture=neutral,
                                                                                                    20 50 75 62 6C 69 63 4B 65 79 54 6F 6B 65 6E 3D   //  PublicKeyToken=
                                                                                                    62 37 37 61 35 63 35 36 31 39 33 34 65 30 38 39   // b77a5c561934e089
                                                                                                    0A 44 61 74 61 41 63 63 65 73 73 00 00 00 00 54   // .DataAccess....T
                                                                                                    02 0F 49 73 44 65 74 65 72 6D 69 6E 69 73 74 69   // ..IsDeterministi
                                                                                                    63 01 54 02 09 49 73 50 72 65 63 69 73 65 01 54   // c.T..IsPrecise.T
                                                                                                    0E 04 4E 61 6D 65 10 75 66 6E 5F 52 65 67 45 78   // ..Name.ufn_RegEx
                                                                                                    49 73 4D 61 74 63 68 54 55 7F 4D 69 63 72 6F 73   // IsMatchTU.Micros
                                                                                                    6F 66 74 2E 53 71 6C 53 65 72 76 65 72 2E 53 65   // oft.SqlServer.Se
                                                                                                    72 76 65 72 2E 53 79 73 74 65 6D 44 61 74 61 41   // rver.SystemDataA
                                                                                                    63 63 65 73 73 4B 69 6E 64 2C 20 53 79 73 74 65   // ccessKind, Syste
                                                                                                    6D 2E 44 61 74 61 2C 20 56 65 72 73 69 6F 6E 3D   // m.Data, Version=
                                                                                                    32 2E 30 2E 30 2E 30 2C 20 43 75 6C 74 75 72 65   // 2.0.0.0, Culture
                                                                                                    3D 6E 65 75 74 72 61 6C 2C 20 50 75 62 6C 69 63   // =neutral, Public
                                                                                                    4B 65 79 54 6F 6B 65 6E 3D 62 37 37 61 35 63 35   // KeyToken=b77a5c5
                                                                                                    36 31 39 33 34 65 30 38 39 10 53 79 73 74 65 6D   // 61934e089.System
                                                                                                    44 61 74 61 41 63 63 65 73 73 00 00 00 00 )       // DataAccess....
    // Code size       62 (0x3e)
    .maxstack  8
    IL_0000:  ldarga.s   input
    IL_0002:  call       instance bool [System.Data]System.Data.SqlTypes.SqlString::get_IsNull()
    IL_0007:  brtrue.s   IL_0012

    IL_0009:  ldarga.s   pattern
    IL_000b:  call       instance bool [System.Data]System.Data.SqlTypes.SqlString::get_IsNull()
    IL_0010:  brfalse.s  IL_0018

    IL_0012:  ldsfld     valuetype [System.Data]System.Data.SqlTypes.SqlBoolean [System.Data]System.Data.SqlTypes.SqlBoolean::Null
    IL_0017:  ret

    IL_0018:  ldarga.s   input
    IL_001a:  call       instance string [System.Data]System.Data.SqlTypes.SqlString::get_Value()
    IL_001f:  ldarga.s   pattern
    IL_0021:  call       instance string [System.Data]System.Data.SqlTypes.SqlString::get_Value()
    IL_0026:  ldarga.s   ignoreCase
    IL_0028:  call       instance bool [System.Data]System.Data.SqlTypes.SqlBoolean::get_Value()
    IL_002d:  brtrue.s   IL_0032

    IL_002f:  ldc.i4.0
    IL_0030:  br.s       IL_0033

    IL_0032:  ldc.i4.1
    IL_0033:  call       bool [System]System.Text.RegularExpressions.Regex::IsMatch(string,
                                                                                    string,
                                                                                    valuetype [System]System.Text.RegularExpressions.RegexOptions)
    IL_0038:  newobj     instance void [System.Data]System.Data.SqlTypes.SqlBoolean::.ctor(bool)
    IL_003d:  ret
  } // end of method SqlRegEx::RegExIsMatch

  .method public hidebysig static valuetype [System.Data]System.Data.SqlTypes.SqlString 
          RegExReplace(valuetype [System.Data]System.Data.SqlTypes.SqlString input,
                       valuetype [System.Data]System.Data.SqlTypes.SqlString pattern,
                       valuetype [System.Data]System.Data.SqlTypes.SqlString replacement,
                       valuetype [System.Data]System.Data.SqlTypes.SqlBoolean ignoreCase) cil managed
  {
    .custom instance void [System.Data]Microsoft.SqlServer.Server.SqlFunctionAttribute::.ctor() = ( 01 00 05 00 54 55 79 4D 69 63 72 6F 73 6F 66 74   // ....TUyMicrosoft
                                                                                                    2E 53 71 6C 53 65 72 76 65 72 2E 53 65 72 76 65   // .SqlServer.Serve
                                                                                                    72 2E 44 61 74 61 41 63 63 65 73 73 4B 69 6E 64   // r.DataAccessKind
                                                                                                    2C 20 53 79 73 74 65 6D 2E 44 61 74 61 2C 20 56   // , System.Data, V
                                                                                                    65 72 73 69 6F 6E 3D 32 2E 30 2E 30 2E 30 2C 20   // ersion=2.0.0.0, 
                                                                                                    43 75 6C 74 75 72 65 3D 6E 65 75 74 72 61 6C 2C   // Culture=neutral,
                                                                                                    20 50 75 62 6C 69 63 4B 65 79 54 6F 6B 65 6E 3D   //  PublicKeyToken=
                                                                                                    62 37 37 61 35 63 35 36 31 39 33 34 65 30 38 39   // b77a5c561934e089
                                                                                                    0A 44 61 74 61 41 63 63 65 73 73 00 00 00 00 54   // .DataAccess....T
                                                                                                    02 0F 49 73 44 65 74 65 72 6D 69 6E 69 73 74 69   // ..IsDeterministi
                                                                                                    63 01 54 02 09 49 73 50 72 65 63 69 73 65 01 54   // c.T..IsPrecise.T
                                                                                                    0E 04 4E 61 6D 65 10 75 66 6E 5F 52 65 67 45 78   // ..Name.ufn_RegEx
                                                                                                    52 65 70 6C 61 63 65 54 55 7F 4D 69 63 72 6F 73   // ReplaceTU.Micros
                                                                                                    6F 66 74 2E 53 71 6C 53 65 72 76 65 72 2E 53 65   // oft.SqlServer.Se
                                                                                                    72 76 65 72 2E 53 79 73 74 65 6D 44 61 74 61 41   // rver.SystemDataA
                                                                                                    63 63 65 73 73 4B 69 6E 64 2C 20 53 79 73 74 65   // ccessKind, Syste
                                                                                                    6D 2E 44 61 74 61 2C 20 56 65 72 73 69 6F 6E 3D   // m.Data, Version=
                                                                                                    32 2E 30 2E 30 2E 30 2C 20 43 75 6C 74 75 72 65   // 2.0.0.0, Culture
                                                                                                    3D 6E 65 75 74 72 61 6C 2C 20 50 75 62 6C 69 63   // =neutral, Public
                                                                                                    4B 65 79 54 6F 6B 65 6E 3D 62 37 37 61 35 63 35   // KeyToken=b77a5c5
                                                                                                    36 31 39 33 34 65 30 38 39 10 53 79 73 74 65 6D   // 61934e089.System
                                                                                                    44 61 74 61 41 63 63 65 73 73 00 00 00 00 )       // DataAccess....
    // Code size       69 (0x45)
    .maxstack  5
    IL_0000:  ldarga.s   input
    IL_0002:  call       instance bool [System.Data]System.Data.SqlTypes.SqlString::get_IsNull()
    IL_0007:  brtrue.s   IL_0012

    IL_0009:  ldarga.s   pattern
    IL_000b:  call       instance bool [System.Data]System.Data.SqlTypes.SqlString::get_IsNull()
    IL_0010:  brfalse.s  IL_0018

    IL_0012:  ldsfld     valuetype [System.Data]System.Data.SqlTypes.SqlString [System.Data]System.Data.SqlTypes.SqlString::Null
    IL_0017:  ret

    IL_0018:  ldarga.s   input
    IL_001a:  call       instance string [System.Data]System.Data.SqlTypes.SqlString::get_Value()
    IL_001f:  ldarga.s   pattern
    IL_0021:  call       instance string [System.Data]System.Data.SqlTypes.SqlString::get_Value()
    IL_0026:  ldarga.s   replacement
    IL_0028:  call       instance string [System.Data]System.Data.SqlTypes.SqlString::get_Value()
    IL_002d:  ldarga.s   ignoreCase
    IL_002f:  call       instance bool [System.Data]System.Data.SqlTypes.SqlBoolean::get_Value()
    IL_0034:  brtrue.s   IL_0039

    IL_0036:  ldc.i4.0
    IL_0037:  br.s       IL_003a

    IL_0039:  ldc.i4.1
    IL_003a:  call       string [System]System.Text.RegularExpressions.Regex::Replace(string,
                                                                                      string,
                                                                                      string,
                                                                                      valuetype [System]System.Text.RegularExpressions.RegexOptions)
    IL_003f:  newobj     instance void [System.Data]System.Data.SqlTypes.SqlString::.ctor(string)
    IL_0044:  ret
  } // end of method SqlRegEx::RegExReplace

  .method public hidebysig static class [mscorlib]System.Collections.IEnumerable 
          RegExMatches(valuetype [System.Data]System.Data.SqlTypes.SqlString input,
                       valuetype [System.Data]System.Data.SqlTypes.SqlString pattern,
                       valuetype [System.Data]System.Data.SqlTypes.SqlBoolean ignoreCase) cil managed
  {
    .custom instance void [System.Data]Microsoft.SqlServer.Server.SqlFunctionAttribute::.ctor() = ( 01 00 06 00 54 55 79 4D 69 63 72 6F 73 6F 66 74   // ....TUyMicrosoft
                                                                                                    2E 53 71 6C 53 65 72 76 65 72 2E 53 65 72 76 65   // .SqlServer.Serve
                                                                                                    72 2E 44 61 74 61 41 63 63 65 73 73 4B 69 6E 64   // r.DataAccessKind
                                                                                                    2C 20 53 79 73 74 65 6D 2E 44 61 74 61 2C 20 56   // , System.Data, V
                                                                                                    65 72 73 69 6F 6E 3D 32 2E 30 2E 30 2E 30 2C 20   // ersion=2.0.0.0, 
                                                                                                    43 75 6C 74 75 72 65 3D 6E 65 75 74 72 61 6C 2C   // Culture=neutral,
                                                                                                    20 50 75 62 6C 69 63 4B 65 79 54 6F 6B 65 6E 3D   //  PublicKeyToken=
                                                                                                    62 37 37 61 35 63 35 36 31 39 33 34 65 30 38 39   // b77a5c561934e089
                                                                                                    0A 44 61 74 61 41 63 63 65 73 73 00 00 00 00 54   // .DataAccess....T
                                                                                                    02 0F 49 73 44 65 74 65 72 6D 69 6E 69 73 74 69   // ..IsDeterministi
                                                                                                    63 01 54 02 09 49 73 50 72 65 63 69 73 65 01 54   // c.T..IsPrecise.T
                                                                                                    0E 04 4E 61 6D 65 10 75 66 6E 5F 52 65 67 45 78   // ..Name.ufn_RegEx
                                                                                                    4D 61 74 63 68 65 73 54 55 7F 4D 69 63 72 6F 73   // MatchesTU.Micros
                                                                                                    6F 66 74 2E 53 71 6C 53 65 72 76 65 72 2E 53 65   // oft.SqlServer.Se
                                                                                                    72 76 65 72 2E 53 79 73 74 65 6D 44 61 74 61 41   // rver.SystemDataA
                                                                                                    63 63 65 73 73 4B 69 6E 64 2C 20 53 79 73 74 65   // ccessKind, Syste
                                                                                                    6D 2E 44 61 74 61 2C 20 56 65 72 73 69 6F 6E 3D   // m.Data, Version=
                                                                                                    32 2E 30 2E 30 2E 30 2C 20 43 75 6C 74 75 72 65   // 2.0.0.0, Culture
                                                                                                    3D 6E 65 75 74 72 61 6C 2C 20 50 75 62 6C 69 63   // =neutral, Public
                                                                                                    4B 65 79 54 6F 6B 65 6E 3D 62 37 37 61 35 63 35   // KeyToken=b77a5c5
                                                                                                    36 31 39 33 34 65 30 38 39 10 53 79 73 74 65 6D   // 61934e089.System
                                                                                                    44 61 74 61 41 63 63 65 73 73 00 00 00 00 54 0E   // DataAccess....T.
                                                                                                    11 46 69 6C 6C 52 6F 77 4D 65 74 68 6F 64 4E 61   // .FillRowMethodNa
                                                                                                    6D 65 0F 47 65 74 52 65 67 45 78 4D 61 74 63 68   // me.GetRegExMatch
                                                                                                    65 73 )                                           // es
    // Code size       53 (0x35)
    .maxstack  8
    IL_0000:  ldarga.s   input
    IL_0002:  call       instance bool [System.Data]System.Data.SqlTypes.SqlString::get_IsNull()
    IL_0007:  brtrue.s   IL_0012

    IL_0009:  ldarga.s   pattern
    IL_000b:  call       instance bool [System.Data]System.Data.SqlTypes.SqlString::get_IsNull()
    IL_0010:  brfalse.s  IL_0014

    IL_0012:  ldnull
    IL_0013:  ret

    IL_0014:  ldarga.s   input
    IL_0016:  call       instance string [System.Data]System.Data.SqlTypes.SqlString::get_Value()
    IL_001b:  ldarga.s   pattern
    IL_001d:  call       instance string [System.Data]System.Data.SqlTypes.SqlString::get_Value()
    IL_0022:  ldarga.s   ignoreCase
    IL_0024:  call       instance bool [System.Data]System.Data.SqlTypes.SqlBoolean::get_Value()
    IL_0029:  brtrue.s   IL_002e

    IL_002b:  ldc.i4.0
    IL_002c:  br.s       IL_002f

    IL_002e:  ldc.i4.1
    IL_002f:  call       class [System]System.Text.RegularExpressions.MatchCollection [System]System.Text.RegularExpressions.Regex::Matches(string,
                                                                                                                                            string,
                                                                                                                                            valuetype [System]System.Text.RegularExpressions.RegexOptions)
    IL_0034:  ret
  } // end of method SqlRegEx::RegExMatches

  .method private hidebysig static void  GetRegExMatches(object input,
                                                         [out] valuetype [System.Data]System.Data.SqlTypes.SqlString& match,
                                                         [out] valuetype [System.Data]System.Data.SqlTypes.SqlInt32& matchIndex,
                                                         [out] valuetype [System.Data]System.Data.SqlTypes.SqlInt32& matchLength) cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class [System]System.Text.RegularExpressions.Match V_0)
    IL_0000:  ldarg.0
    IL_0001:  castclass  [System]System.Text.RegularExpressions.Match
    IL_0006:  stloc.0
    IL_0007:  ldarg.1
    IL_0008:  ldloc.0
    IL_0009:  callvirt   instance string [System]System.Text.RegularExpressions.Capture::get_Value()
    IL_000e:  newobj     instance void [System.Data]System.Data.SqlTypes.SqlString::.ctor(string)
    IL_0013:  stobj      [System.Data]System.Data.SqlTypes.SqlString
    IL_0018:  ldarg.2
    IL_0019:  ldloc.0
    IL_001a:  callvirt   instance int32 [System]System.Text.RegularExpressions.Capture::get_Index()
    IL_001f:  newobj     instance void [System.Data]System.Data.SqlTypes.SqlInt32::.ctor(int32)
    IL_0024:  stobj      [System.Data]System.Data.SqlTypes.SqlInt32
    IL_0029:  ldarg.3
    IL_002a:  ldloc.0
    IL_002b:  callvirt   instance int32 [System]System.Text.RegularExpressions.Capture::get_Length()
    IL_0030:  newobj     instance void [System.Data]System.Data.SqlTypes.SqlInt32::.ctor(int32)
    IL_0035:  stobj      [System.Data]System.Data.SqlTypes.SqlInt32
    IL_003a:  ret
  } // end of method SqlRegEx::GetRegExMatches

  .method public hidebysig static class [mscorlib]System.Collections.IEnumerable 
          RegExSplit(valuetype [System.Data]System.Data.SqlTypes.SqlString input,
                     valuetype [System.Data]System.Data.SqlTypes.SqlString pattern,
                     valuetype [System.Data]System.Data.SqlTypes.SqlBoolean ignoreCase) cil managed
  {
    .custom instance void [System.Data]Microsoft.SqlServer.Server.SqlFunctionAttribute::.ctor() = ( 01 00 06 00 54 55 79 4D 69 63 72 6F 73 6F 66 74   // ....TUyMicrosoft
                                                                                                    2E 53 71 6C 53 65 72 76 65 72 2E 53 65 72 76 65   // .SqlServer.Serve
                                                                                                    72 2E 44 61 74 61 41 63 63 65 73 73 4B 69 6E 64   // r.DataAccessKind
                                                                                                    2C 20 53 79 73 74 65 6D 2E 44 61 74 61 2C 20 56   // , System.Data, V
                                                                                                    65 72 73 69 6F 6E 3D 32 2E 30 2E 30 2E 30 2C 20   // ersion=2.0.0.0, 
                                                                                                    43 75 6C 74 75 72 65 3D 6E 65 75 74 72 61 6C 2C   // Culture=neutral,
                                                                                                    20 50 75 62 6C 69 63 4B 65 79 54 6F 6B 65 6E 3D   //  PublicKeyToken=
                                                                                                    62 37 37 61 35 63 35 36 31 39 33 34 65 30 38 39   // b77a5c561934e089
                                                                                                    0A 44 61 74 61 41 63 63 65 73 73 00 00 00 00 54   // .DataAccess....T
                                                                                                    02 0F 49 73 44 65 74 65 72 6D 69 6E 69 73 74 69   // ..IsDeterministi
                                                                                                    63 01 54 02 09 49 73 50 72 65 63 69 73 65 01 54   // c.T..IsPrecise.T
                                                                                                    0E 04 4E 61 6D 65 0E 75 66 6E 5F 52 65 67 45 78   // ..Name.ufn_RegEx
                                                                                                    53 70 6C 69 74 54 55 7F 4D 69 63 72 6F 73 6F 66   // SplitTU.Microsof
                                                                                                    74 2E 53 71 6C 53 65 72 76 65 72 2E 53 65 72 76   // t.SqlServer.Serv
                                                                                                    65 72 2E 53 79 73 74 65 6D 44 61 74 61 41 63 63   // er.SystemDataAcc
                                                                                                    65 73 73 4B 69 6E 64 2C 20 53 79 73 74 65 6D 2E   // essKind, System.
                                                                                                    44 61 74 61 2C 20 56 65 72 73 69 6F 6E 3D 32 2E   // Data, Version=2.
                                                                                                    30 2E 30 2E 30 2C 20 43 75 6C 74 75 72 65 3D 6E   // 0.0.0, Culture=n
                                                                                                    65 75 74 72 61 6C 2C 20 50 75 62 6C 69 63 4B 65   // eutral, PublicKe
                                                                                                    79 54 6F 6B 65 6E 3D 62 37 37 61 35 63 35 36 31   // yToken=b77a5c561
                                                                                                    39 33 34 65 30 38 39 10 53 79 73 74 65 6D 44 61   // 934e089.SystemDa
                                                                                                    74 61 41 63 63 65 73 73 00 00 00 00 54 0E 11 46   // taAccess....T..F
                                                                                                    69 6C 6C 52 6F 77 4D 65 74 68 6F 64 4E 61 6D 65   // illRowMethodName
                                                                                                    0E 47 65 74 52 65 67 45 78 53 70 6C 69 74 73 )    // .GetRegExSplits
    // Code size       53 (0x35)
    .maxstack  8
    IL_0000:  ldarga.s   input
    IL_0002:  call       instance bool [System.Data]System.Data.SqlTypes.SqlString::get_IsNull()
    IL_0007:  brtrue.s   IL_0012

    IL_0009:  ldarga.s   pattern
    IL_000b:  call       instance bool [System.Data]System.Data.SqlTypes.SqlString::get_IsNull()
    IL_0010:  brfalse.s  IL_0014

    IL_0012:  ldnull
    IL_0013:  ret

    IL_0014:  ldarga.s   input
    IL_0016:  call       instance string [System.Data]System.Data.SqlTypes.SqlString::get_Value()
    IL_001b:  ldarga.s   pattern
    IL_001d:  call       instance string [System.Data]System.Data.SqlTypes.SqlString::get_Value()
    IL_0022:  ldarga.s   ignoreCase
    IL_0024:  call       instance bool [System.Data]System.Data.SqlTypes.SqlBoolean::get_Value()
    IL_0029:  brtrue.s   IL_002e

    IL_002b:  ldc.i4.0
    IL_002c:  br.s       IL_002f

    IL_002e:  ldc.i4.1
    IL_002f:  call       string[] [System]System.Text.RegularExpressions.Regex::Split(string,
                                                                                      string,
                                                                                      valuetype [System]System.Text.RegularExpressions.RegexOptions)
    IL_0034:  ret
  } // end of method SqlRegEx::RegExSplit

  .method private hidebysig static void  GetRegExSplits(object input,
                                                        [out] valuetype [System.Data]System.Data.SqlTypes.SqlString& match) cil managed
  {
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.0
    IL_0002:  castclass  [mscorlib]System.String
    IL_0007:  newobj     instance void [System.Data]System.Data.SqlTypes.SqlString::.ctor(string)
    IL_000c:  stobj      [System.Data]System.Data.SqlTypes.SqlString
    IL_0011:  ret
  } // end of method SqlRegEx::GetRegExSplits

  .method private hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method SqlRegEx::.ctor

} // end of class SqlClrTools.SqlRegEx


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\Users\mjensen\projects\SqlRegEx\bin\Debug\SqlRegEx.res
