Imports System.CodeDom.Compiler
Imports System.Numerics
Imports System.Reflection
Imports System.Runtime.InteropServices
Imports System.Text

Imports FoxScript.Evaluator
Imports FoxScript.Utils
Imports FoxScript.BooleanUtils
Imports FoxScript.FileSystemUtils
Imports FoxScript.StringUtils
Imports FoxScript.ErrorUtils
Imports FoxScript.StatementUtils

Public Class Builtins
    Public Shared AppPath = AppDomain.CurrentDomain.BaseDirectory
    Public Shared builtinFuncs As New Dictionary(Of String, Fox_Builtin) From
    {
        {"GetObjectType", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                                       Dim 所需实参数 = 1

                                                                       If args.Count > 所需实参数 Then
                                                                           Return ThrowError($"实参过多")
                                                                       End If

                                                                       If args.Count < 所需实参数 Then
                                                                           Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                                       End If

                                                                       If args.Count = 所需实参数 Then
                                                                           Dim Obj As Fox_Object = args(0)
                                                                           If Obj.Type = ObjectType.CLASS_OBJ Then
                                                                               Return New Fox_String With {.Value = Trim(DirectCast(Obj, Fox_Class).Name.Value)}
                                                                           End If

                                                                           Return New Fox_String With {.Value = Obj.Type.ToString}
                                                                       End If


                                                                       Return Nothing
                                                                   End Function}},
        {"input", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                               Dim 所需实参数 = 1

                                                               If args.Count > 所需实参数 Then
                                                                   Return ThrowError($"实参过多")
                                                               End If

                                                               If args.Count = 所需实参数 Then
                                                                   Console.Write(args(0).Value)
                                                                   Dim r = Console.ReadLine
                                                                   Return New Fox_String With {.Value = r}
                                                               End If

                                                               If Not args.Count Then
                                                                   Dim r = Console.ReadLine()
                                                                   Return New Fox_String With {.Value = r}
                                                               End If

                                                               If args.Count < 所需实参数 Then
                                                                   Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                               End If

                                                               Return Nothing
                                                           End Function}},
        {"len", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                             Dim 所需实参数 = 1

                                                             If args.Count > 所需实参数 Then
                                                                 Return ThrowError($"实参过多")
                                                             End If

                                                             If args.Count < 所需实参数 Then
                                                                 Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                             End If

                                                             If args.Count = 所需实参数 Then
                                                                 Select Case args(0).Type()
                                                                     Case ObjectType.STRING_OBJ
                                                                         Return New Fox_Integer With {.Value = CLng(args(0).Value.ToString.Count)}
                                                                     Case ObjectType.ARRAY_OBJ
                                                                         Return New Fox_Integer With {.Value = CLng(TryCast(args(0), Fox_Array).Elements.Count)}
                                                                     Case Else
                                                                         Return ThrowError($"len函数不支持{args(0).Type()}类型")
                                                                 End Select
                                                             End If

                                                             Return Nothing
                                                         End Function}},
        {"print", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                               Dim 所需实参数 = 1

                                                               If args.Count > 所需实参数 Then
                                                                   Return ThrowError($"实参过多")
                                                               End If

                                                               If args.Count = 所需实参数 AndAlso args(0) IsNot Nothing Then
                                                                   If TypeOf args(0) Is Fox_String Then
                                                                       Console.WriteLine(args(0).Value)
                                                                   Else
                                                                       Console.WriteLine(args(0).Inspect)
                                                                   End If
                                                               End If

                                                               If args.Count < 所需实参数 Then
                                                                   Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                               End If

                                                               Return Nothing
                                                           End Function}},
        {"RunVBCode", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                                   Dim 所需实参数 = 1

                                                                   If args.Count > 所需实参数 Then
                                                                       Return ThrowError($"实参过多")
                                                                   End If

                                                                   If args.Count = 所需实参数 AndAlso args(0) IsNot Nothing Then
                                                                       If TypeOf args(0) Is Fox_String Then
                                                                           ' 创建一个代码编译器
                                                                           Dim provider As CodeDomProvider = CodeDomProvider.CreateProvider("VisualBasic")
                                                                           Dim compilerParams As New CompilerParameters()

                                                                           ' 添加引用
                                                                           compilerParams.ReferencedAssemblies.Add("System.dll")

                                                                           ' 设置编译选项
                                                                           compilerParams.GenerateExecutable = False
                                                                           compilerParams.GenerateInMemory = True

                                                                           ' 定义要编译的代码
                                                                           Dim code As String = args(0).Value
                                                                           ' 编译代码
                                                                           Dim results As CompilerResults = provider.CompileAssemblyFromSource(compilerParams, code)

                                                                           ' 检查是否有编译错误
                                                                           If results.Errors.Count > 0 Then
                                                                               Return New Fox_String With {.Value = results.Errors(0).ToString}
                                                                           Else
                                                                               ' 执行 Main 方法
                                                                               Dim moduleType As Type = results.CompiledAssembly.GetType("Module1")
                                                                               Dim mainMethod As MethodInfo = moduleType.GetMethod("Main")
                                                                               mainMethod.Invoke(Nothing, Nothing)
                                                                           End If
                                                                       Else
                                                                           Return ThrowError($"类型错误: 参数{1}类型为{args(0).Type()}")
                                                                       End If
                                                                   End If

                                                                   If args.Count < 所需实参数 Then
                                                                       Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                                   End If


                                                                   Return Nothing
                                                               End Function}},
        {"msgbox", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                                Dim 所需实参数 = 2

                                                                If args.Count > 所需实参数 Then
                                                                    Return ThrowError($"实参过多 实参{args.Count}个 形参{所需实参数}个")
                                                                End If

                                                                If args.Count < 所需实参数 Then
                                                                    Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                                End If

                                                                If args.Count = 所需实参数 Then
                                                                    Dim msg, title
                                                                    Select Case args(0).GetType()
                                                                        Case GetType(Fox_String)
                                                                            msg = args(0).Value
                                                                        Case GetType(Fox_Integer)
                                                                            msg = args(0).Value.ToString
                                                                        Case GetType(Fox_Bool)
                                                                            msg = args(0).Value.ToString
                                                                        Case Else
                                                                            Return ThrowError($"MsgBox函数不支持{args(0).Type()}类型")
                                                                    End Select

                                                                    Select Case args(1).GetType()
                                                                        Case GetType(Fox_String)
                                                                            title = args(1).Value
                                                                        Case GetType(Fox_Integer)
                                                                            title = args(1).Value.ToString
                                                                        Case GetType(Fox_Bool)
                                                                            title = args(1).Value.ToString
                                                                        Case Else
                                                                            Return ThrowError($"MsgBox函数不支持{args(1).Type()}类型")
                                                                    End Select

                                                                    MsgBox(msg,, title)
                                                                End If

                                                                Return Obj_Nothing
                                                            End Function}},
        {"clear", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                               Dim 所需实参数 = 0
                                                               If args.Count > 所需实参数 Then
                                                                   Return ThrowError($"实参过多 实参{args.Count}个 形参{所需实参数}个")
                                                               End If

                                                               If args.Count = 所需实参数 Then
                                                                   Console.Clear()
                                                               End If

                                                               Return Nothing
                                                           End Function}},
        {"exit", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                              End
                                                          End Function}},
        {"CDbl", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                              Dim 所需实参数 = 1

                                                              If args.Count > 所需实参数 Then
                                                                  Return ThrowError($"实参过多 实参{args.Count}个 形参{所需实参数}个")
                                                              End If

                                                              If args.Count < 所需实参数 Then
                                                                  Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                              End If
                                                              If args.Count = 所需实参数 Then
                                                                  Select Case args(0).Type()
                                                                      Case ObjectType.INTEGER_OBJ
                                                                          Return New Fox_Double With {.Value = CDbl(CLng(args(0).Value.ToString()))}
                                                                      Case ObjectType.DOUBLE_OBJ
                                                                          Return args(0)
                                                                      Case ObjectType.BOOL_OBJ
                                                                          Return New Fox_Double With {.Value = CDbl(Evaluator.BoolToLong(args(0).Value))}
                                                                      Case ObjectType.STRING_OBJ
                                                                          Dim strObj = TryCast(args(0), Fox_String)
                                                                          Dim r = Nothing
                                                                          If Decimal.TryParse(strObj.Value, r) Then
                                                                              Return New Fox_Double With {.Value = r}
                                                                          Else
                                                                              Return ThrowError($"无法转换 {strObj.Value} 为小数")
                                                                          End If
                                                                      Case ObjectType.NOTHINGL_OBJ
                                                                          Return New Fox_Double With {.Value = 0.0}
                                                                      Case Else
                                                                          Return ThrowError($"CDbl函数不支持{args(0).Type()}类型")
                                                                  End Select
                                                              End If

                                                              Return Obj_Nothing
                                                          End Function}},
        {"CInt", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                              Dim 所需实参数 = 1

                                                              If args.Count > 所需实参数 Then
                                                                  Return ThrowError($"实参过多 实参{args.Count}个 形参{所需实参数}个")
                                                              End If

                                                              If args.Count < 所需实参数 Then
                                                                  Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                              End If

                                                              If args.Count = 所需实参数 Then
                                                                  Select Case args(0).Type()
                                                                      Case ObjectType.INTEGER_OBJ
                                                                          Return args(0)
                                                                      Case ObjectType.DOUBLE_OBJ
                                                                          Return New Fox_Integer With {.Value = New BigInteger(CLng(args(0).Value))}
                                                                      Case ObjectType.BOOL_OBJ
                                                                          Return New Fox_Integer With {.Value = Evaluator.BoolToLong(args(0).Value)}

                                                                      Case ObjectType.STRING_OBJ
                                                                          Dim strObj = TryCast(args(0), Fox_String)
                                                                          Dim r As Long = Nothing
                                                                          Dim strValue = strObj.Value

                                                                          ' 检查是否为十六进制字符串
                                                                          If strValue.StartsWith("&H") OrElse strValue.StartsWith("&h") OrElse strValue.StartsWith("0x") Then
                                                                              Dim hexValue = strValue.Substring(2) ' 移除前缀
                                                                              Try
                                                                                  r = Convert.ToInt64(hexValue, 16) ' 将十六进制字符串转换为Long
                                                                                  Return New Fox_Integer With {.Value = New BigInteger(r)}
                                                                              Catch ex As Exception
                                                                                  Return ThrowError($"无法转换 {strObj.Value} 为整数")
                                                                              End Try
                                                                          Else
                                                                              ' 尝试将字符串转换为Long
                                                                              If Long.TryParse(strValue, r) Then
                                                                                  Return New Fox_Integer With {.Value = r}
                                                                              Else
                                                                                  Return ThrowError($"无法转换 {strObj.Value} 为整数")
                                                                              End If
                                                                          End If
                                                                      Case ObjectType.NOTHINGL_OBJ
                                                                          Return Fox_Zero
                                                                      Case Else
                                                                          Return ThrowError($"CInt函数不支持{args(0).Type()}类型")
                                                                  End Select
                                                              End If

                                                              Return Obj_Nothing
                                                          End Function}},
        {"CBool", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                               Dim 所需实参数 = 1

                                                               If args.Count > 所需实参数 Then
                                                                   Return ThrowError($"实参过多 实参{args.Count}个 形参{所需实参数}个")
                                                               End If

                                                               If args.Count < 所需实参数 Then
                                                                   Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                               End If

                                                               If args.Count = 所需实参数 Then
                                                                   If args(0) Is Nothing Then Return ThrowError($"CBool函数不支{Obj_Nothing.Type}类型")

                                                                   Select Case TryCast(args(0), Fox_Object).Type
                                                                       Case ObjectType.INTEGER_OBJ, ObjectType.DOUBLE_OBJ
                                                                           Return New Fox_Bool With {.Value = args(0).Value <> 0}
                                                                       Case ObjectType.BOOL_OBJ
                                                                           Return args(0)
                                                                       Case ObjectType.STRING_OBJ
                                                                           Return If(TryCast(args(0), Fox_String).Value <> "", Fox_True, Fox_False)
                                                                       Case ObjectType.ARRAY_OBJ
                                                                           Return New Fox_Bool With {.Value = TryCast(args(0), Fox_Array).Elements.Any}
                                                                       Case ObjectType.DICTIONARY_OBJ
                                                                           Return New Fox_Bool With {.Value = TryCast(args(0), Fox_Dictionary).Pairs.Any}
                                                                       Case ObjectType.NOTHINGL_OBJ
                                                                           Return Fox_False
                                                                       Case Else
                                                                           Return ThrowError($"CInt函数不支持{args(0).Type()}类型")
                                                                   End Select
                                                               End If

                                                               Return Obj_Nothing
                                                           End Function}},
        {"CStr", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                              Dim 所需实参数 = 1

                                                              If args.Count > 所需实参数 Then
                                                                  Return ThrowError($"实参过多 实参{args.Count}个 形参{所需实参数}个")
                                                              End If

                                                              If args.Count < 所需实参数 Then
                                                                  Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                              End If

                                                              If args.Count = 所需实参数 Then
                                                                  Select Case args(0).Type()
                                                                      Case ObjectType.INTEGER_OBJ, ObjectType.DOUBLE_OBJ
                                                                          Return New Fox_String With {.Value = args(0).Value.ToString}
                                                                      Case ObjectType.BOOL_OBJ
                                                                          Return New Fox_String With {.Value = CStr(args(0).Value)}
                                                                      Case ObjectType.STRING_OBJ
                                                                          Return args(0)
                                                                      Case ObjectType.NOTHINGL_OBJ
                                                                          Return New Fox_String With {.Value = ""}
                                                                      Case Else
                                                                          Return ThrowError($"CStr函数不支持{args(0).Type()}类型")
                                                                  End Select
                                                              End If

                                                              Return Obj_Nothing
                                                          End Function}},
        {"CArray", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                                Dim 所需实参数 = 1

                                                                If args.Count > 所需实参数 Then
                                                                    Return ThrowError($"实参过多 实参{args.Count}个 形参{所需实参数}个")
                                                                End If

                                                                If args.Count < 所需实参数 Then
                                                                    Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                                End If

                                                                If args.Count = 所需实参数 Then
                                                                    Select Case args(0).Type()
                                                                        Case ObjectType.ARRAY_OBJ
                                                                            Return args(0)
                                                                        Case ObjectType.NOTHINGL_OBJ
                                                                            Return New Fox_Array With {.Elements = New List(Of Fox_Object)}
                                                                        Case Else
                                                                            Return ThrowError($"CStr函数不支持{args(0).Type()}类型")
                                                                    End Select
                                                                End If

                                                                Return Obj_Nothing
                                                            End Function}},
        {"range", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                               Dim 所需实参数 = 1

                                                               If args.Count > 所需实参数 Then
                                                                   Return ThrowError($"实参过多 实参{args.Count}个 形参{所需实参数}个")
                                                               End If

                                                               If args.Count < 所需实参数 Then
                                                                   Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                               End If

                                                               If args.Count = 所需实参数 Then
                                                                   Select Case args(0).Type()
                                                                       Case ObjectType.INTEGER_OBJ
                                                                           Dim arr = New Fox_Array With {.Elements = New List(Of Fox_Object)}
                                                                           Try
                                                                               For i = 0 To args(0).Value - 1
                                                                                   arr.Elements.Add(New Fox_Integer With {.Value = i})
                                                                               Next
                                                                           Catch ex As OutOfMemoryException
                                                                               Return ThrowError($"内存溢出! {ex.Message}")
                                                                           End Try
                                                                           Return arr
                                                                       Case Else
                                                                           Return ThrowError($"range函数不支持{args(0).Type()}类型")
                                                                   End Select
                                                               End If

                                                               Return Obj_Nothing
                                                           End Function}},
        {"Sort", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                              Dim 所需实参数 = 1

                                                              If args.Count > 所需实参数 Then
                                                                  Return ThrowError($"实参过多 实参{args.Count}个 形参{所需实参数}个")
                                                              End If

                                                              If args.Count < 所需实参数 Then
                                                                  Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                              End If

                                                              If args.Count = 所需实参数 Then
                                                                  Select Case args(0).Type()
                                                                      Case ObjectType.ARRAY_OBJ
                                                                          Dim arr = TryCast(args(0), Fox_Array)
                                                                          Dim numbers As New List(Of BigInteger)

                                                                          For Each item As Fox_Object In arr.Elements
                                                                              numbers.Add(builtinFuncs("CInt").BuiltinFunction({item}).Value)
                                                                          Next

                                                                          numbers.Sort()

                                                                          Dim objects As New List(Of Fox_Object)
                                                                          For i = 0 To numbers.Count - 1
                                                                              Dim sorted_number As Long = numbers(i)
                                                                              objects.Add(New Fox_Integer With {.Value = New BigInteger(sorted_number)})
                                                                          Next

                                                                          Return New Fox_Array With {.Elements = objects}
                                                                      Case Else
                                                                          Return ThrowError($"BubbleSort函数不支持{args(0).Type()}类型")
                                                                  End Select
                                                              End If

                                                              Return Obj_Nothing
                                                          End Function}},
        {"CNothing", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                                  Dim 所需实参数 = 1

                                                                  If args.Count > 所需实参数 Then
                                                                      Return ThrowError($"实参过多 实参{args.Count}个 形参{所需实参数}个")
                                                                  End If

                                                                  If args.Count < 所需实参数 Then
                                                                      Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                                  End If

                                                                  Return Obj_Nothing
                                                              End Function}},
        {"Chr", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                             Dim 所需实参数 = 1

                                                             If args.Count > 所需实参数 Then
                                                                 Return ThrowError($"实参过多 实参{args.Count}个 形参{所需实参数}个")
                                                             End If

                                                             If args.Count < 所需实参数 Then
                                                                 Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                             End If

                                                             If args.Count = 所需实参数 Then
                                                                 Select Case args(0).Type()
                                                                     Case ObjectType.INTEGER_OBJ
                                                                         Return New Fox_String With {.Value = Chr(CLng(args(0).Value.ToString))}
                                                                     Case Else
                                                                         Return ThrowError($"Chr函数不支持{args(0).Type()}类型")
                                                                 End Select
                                                             End If

                                                             Return Obj_Nothing
                                                         End Function}},
        {"Asc", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                             Dim 所需实参数 = 1

                                                             If args.Count > 所需实参数 Then
                                                                 Return ThrowError($"实参过多 实参{args.Count}个 形参{所需实参数}个")
                                                             End If

                                                             If args.Count < 所需实参数 Then
                                                                 Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                             End If

                                                             If args.Count = 所需实参数 Then

                                                                 Select Case args(0).Type()
                                                                     Case ObjectType.STRING_OBJ
                                                                         Return New Fox_Integer With {.Value = Asc(CChar(args(0).Value))}
                                                                     Case Else
                                                                         Return ThrowError($"Asc函数不支持{args(0).Type()}类型")
                                                                 End Select
                                                             End If

                                                             Return Obj_Nothing
                                                         End Function}},
        {"AscW", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                              Dim 所需实参数 = 1

                                                              If args.Count > 所需实参数 Then
                                                                  Return ThrowError($"实参过多 实参{args.Count}个 形参{所需实参数}个")
                                                              End If

                                                              If args.Count < 所需实参数 Then
                                                                  Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                              End If

                                                              If args.Count = 所需实参数 Then
                                                                  Select Case args(0).Type()
                                                                      Case ObjectType.STRING_OBJ
                                                                          Return New Fox_Integer With {.Value = AscW(CChar(args(0).Value))}
                                                                      Case Else
                                                                          Return ThrowError($"AscW函数不支持{args(0).Type()}类型")
                                                                  End Select
                                                              End If

                                                              Return Obj_Nothing
                                                          End Function}},
        {"ChrW", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                              Dim 所需实参数 = 1

                                                              If args.Count > 所需实参数 Then
                                                                  Return ThrowError($"实参过多 实参{args.Count}个 形参{所需实参数}个")
                                                              End If

                                                              If args.Count < 所需实参数 Then
                                                                  Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                              End If

                                                              If args.Count = 所需实参数 Then
                                                                  Select Case args(0).Type()
                                                                      Case ObjectType.INTEGER_OBJ
                                                                          Return New Fox_String With {.Value = ChrW(CLng(args(0).Value.ToString))}
                                                                      Case Else
                                                                          Return ThrowError($"ChrW函数不支持{args(0).Type()}类型")
                                                                  End Select
                                                              End If

                                                              Return Obj_Nothing
                                                          End Function}},
        {"Eval", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                              Dim 所需实参数 = 1

                                                              If args.Count > 所需实参数 Then
                                                                  Return ThrowError($"实参过多 实参{args.Count}个 形参{所需实参数}个")
                                                              End If

                                                              If args.Count < 所需实参数 Then
                                                                  Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                              End If

                                                              If args.Count = 所需实参数 Then
                                                                  Select Case args(0).Type()
                                                                      Case ObjectType.STRING_OBJ
                                                                          Return Runner.Eval(args(0).Value, New Environment)
                                                                      Case Else
                                                                          Return ThrowError($"Eval函数不支持{args(0).Type()}类型")
                                                                  End Select
                                                              End If

                                                              Return Obj_Nothing
                                                          End Function}},
        {"replace", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                                 Dim 所需实参数 = 3

                                                                 If args.Count > 所需实参数 Then
                                                                     Return ThrowError($"实参过多 实参{args.Count}个 形参{所需实参数}个")
                                                                 End If

                                                                 If args.Count < 所需实参数 Then
                                                                     Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                                 End If

                                                                 If args.Count = 所需实参数 Then
                                                                     For Each arg As Fox_Object In args
                                                                         If arg.Type <> ObjectType.STRING_OBJ Then
                                                                             Return ThrowError($"类型错误: 在参数{args.ToList.IndexOf(arg) + 1} 中传入了一个{arg.Type}类型的参数")
                                                                         End If
                                                                     Next

                                                                     If args(1).Value = "" Then
                                                                         Return New Fox_String With {.Value = ""}
                                                                     End If

                                                                     Return New Fox_String With {.Value = args(0).Value.ToString.Replace(args(1).Value, args(2).Value)}
                                                                 End If

                                                                 Return Obj_Nothing
                                                             End Function}},
        {"format", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                                Dim 所需实参数 = 2

                                                                If args.Count > 所需实参数 Then
                                                                    Return ThrowError($"实参过多 实参{args.Count}个 形参{所需实参数}个")
                                                                End If

                                                                If args.Count < 所需实参数 Then
                                                                    Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                                End If

                                                                If args.Count = 所需实参数 Then
                                                                    If args(0).Type() <> ObjectType.STRING_OBJ Then
                                                                        Return ThrowError($"类型错误: 在参数{1} 中传入了一个{args(0).Type}类型的参数")
                                                                    ElseIf args(1).Type() <> ObjectType.ARRAY_OBJ Then
                                                                        Return ThrowError($"类型错误: 在参数{2} 中传入了一个{args(1).Type}类型的参数")
                                                                    End If

                                                                    If args(0).Value = "" Then
                                                                        Return args(0)
                                                                    End If

                                                                    Dim string_val = TryCast(args(0), Fox_String).Value
                                                                    Dim format_list = TryCast(args(1), Fox_Array).Elements
                                                                    Dim values As New List(Of String)

                                                                    For Each obj As Object In format_list
                                                                        If TypeOf obj Is Fox_String Then
                                                                            values.Add(obj.Value)
                                                                        Else
                                                                            values.Add(obj.Inspect())
                                                                        End If
                                                                    Next

                                                                    Return New Fox_String With {.Value = Fox_Format(string_val, values)}

                                                                End If

                                                                Return Obj_Nothing
                                                            End Function}},
        {"random", New Fox_Builtin With {.BuiltinFunction = Function(args As IEnumerable(Of Object))
                                                                Dim 所需实参数 = 2

                                                                If args.Count > 所需实参数 Then
                                                                    Return ThrowError($"实参过多")
                                                                End If

                                                                If args.Count = 所需实参数 AndAlso args(0) IsNot Nothing Then
                                                                    If args(0).Type() <> ObjectType.INTEGER_OBJ Then
                                                                        Return ThrowError($"类型错误: 在参数{1} 中传入了一个{args(0).Type}类型的参数")
                                                                    ElseIf args(1).Type() <> ObjectType.INTEGER_OBJ Then
                                                                        Return ThrowError($"类型错误: 在参数{2} 中传入了一个{args(1).Type}类型的参数")
                                                                    End If
                                                                    If args(1).Value < args(0).Value Then
                                                                        Return ThrowError($"参数{1}必须小于等于参数{2}")
                                                                    End If
                                                                    Dim val As BigInteger = Utils.GenerateRandomBigInteger(args(0).Value, args(1).Value)
                                                                    Return New Fox_Integer With {.Value = val}
                                                                End If

                                                                If args.Count < 所需实参数 Then
                                                                    Return ThrowError($"提供的实参过少 实参{args.Count}个 形参{所需实参数}个")
                                                                End If

                                                                Return Nothing
                                                            End Function}}
    }
    Public Shared builtinVars As New Dictionary(Of String, Fox_Builtin) From
    {
        {"Crlf", New Fox_Builtin With {.BuiltinIdentifier = New Fox_String With {.Value = vbCrLf}}},
        {"NextLine", New Fox_Builtin With {.BuiltinIdentifier = New Fox_String With {.Value = vbCrLf}}},
        {"InterpreterPath", New Fox_Builtin With {.BuiltinIdentifier = New Fox_String With {.Value = AppPath}}},
        {"IncludesPath", New Fox_Builtin With {.BuiltinIdentifier = New Fox_String With {.Value = AppPath & "includes\"}}},
        {"Math_Module_Path", New Fox_Builtin With {.BuiltinIdentifier = New Fox_String With {.Value = AppPath & "includes\Math.Fox"}}},
        {"IO_Module_Path", New Fox_Builtin With {.BuiltinIdentifier = New Fox_String With {.Value = AppPath & "includes\IO.Fox"}}},
        {"Nothing", New Fox_Builtin With {.BuiltinIdentifier = New Fox_Nothing}}
    }

    Shared Function Fox_Format(ByVal formatString As String, ByVal replacements As List(Of String)) As String
        Dim formattedString As String = formatString
        Dim placeHolderPattern As String = "{}"

        For i As Integer = 0 To replacements.Count - 1
            Dim placeholderIndex As Integer = formattedString.IndexOf(placeHolderPattern)
            If placeholderIndex <> -1 Then
                formattedString = formattedString.Substring(0, placeholderIndex) & replacements(i) & formattedString.Substring(placeholderIndex + placeHolderPattern.Length)
            End If
        Next

        Return formattedString
    End Function
End Class



