﻿Imports System.IO
Imports FoxScript.Utils
Public Class ErrorUtils
    '引发异常 返回一个Fox_Error对象
    Public Shared Function ThrowError(message As String) As Fox_Error
        '返回
        Return New Fox_Error With {.Message = message}
    End Function
    Public Shared Function CheckObject(operator_ As String, Left As Fox_Object, Right As Fox_Object)
        If Left Is Nothing Then
            If Right Is Nothing Then
                Return ThrowError($"未知的操作: {Obj_Nothing.Type} {operator_} {Obj_Nothing.Type}")
            End If
            Return ThrowError($"未知的操作: {Obj_Nothing.Type} {operator_} {Right.Type}")
        Else
            If Right Is Nothing Then
                Return ThrowError($"未知的操作: {Left.Type} {operator_} {Obj_Nothing.Type}")
            End If
        End If

        If IsError(Left) Then Return Left
        If IsError(Right) Then Return Right

        Return Nothing
    End Function
End Class

Public Class BooleanUtils

    Public Shared Fox_True As New Fox_Bool() With {.Value = True}
    Public Shared Fox_False As New Fox_Bool() With {.Value = False}
    Public Shared Fox_One As New Fox_Integer() With {.Value = 1}
    Public Shared Fox_Zero As New Fox_Integer() With {.Value = 0}

    '将布尔值转换为Fox_Bool对象
    Public Shared Function NativeBoolToBooleanObject(Input As Boolean) As Fox_Bool
        If Input Then
            Return Fox_True
        End If
        Return Fox_False
    End Function
End Class

Public Class StatementUtils
    Public Shared Function FindVBMemberIndex(VBMembers As List(Of VB_Object), Name As String) As ULong
        For memberIndex = 0 To VBMembers.Count - 1
            Dim obj As Object = VBMembers(memberIndex)
            If obj.Name.Value = Name Then
                Return memberIndex
            End If
        Next

        Return Nothing
    End Function

    Public Shared Sub RemoveAllStatement(ByRef OriginStatements As List(Of Statement), Type As Type)
        ' 创建一个要删除的元素的列表
        Dim Removes As New List(Of Statement)

        ' 遍历原始列表，将符合条件的元素添加到Removes列表中
        For Each stmt As Statement In OriginStatements
            If stmt.GetType() Is Type Then
                Removes.Add(stmt)
            End If
        Next

        ' 遍历Removes列表，并从原始列表中删除这些元素
        For Each stmt As Statement In Removes
            OriginStatements.Remove(stmt)
        Next
    End Sub

    Public Shared Function FindAllStatement(OriginStatements As List(Of Statement), Type As Type) As List(Of Statement)
        Dim Statements As New List(Of Statement)
        For Each stmt As Statement In OriginStatements
            If stmt.GetType = Type Then
                Statements.Add(stmt)
            End If
        Next

        Return Statements
    End Function

    Public Shared Function FindAllExpression(OriginExpressions As List(Of ExpressionStatement), Type As Type)
        Dim Expressions As New List(Of Expression)
        For Each exp As ExpressionStatement In OriginExpressions
            If exp.Expression Is Nothing Then Continue For

            If exp.Expression.GetType = Type Then
                Expressions.Add(exp.Expression)
            End If
        Next

        Return Expressions
    End Function

    Public Shared Function FindFunctionLiteral(Name As String, Statements As List(Of Statement)) As FunctionLiteral
        Dim Stmts = FindAllStatement(Statements, GetType(ExpressionStatement))
        Dim expStmts As New List(Of ExpressionStatement)
        For Each stmt As ExpressionStatement In Stmts
            expStmts.Add(stmt)
        Next


        Dim funcLiterals = FindAllExpression(expStmts, GetType(FunctionLiteral))
        For Each stmt As FunctionLiteral In funcLiterals
            Dim funcLiteral = TryCast(stmt, FunctionLiteral)
            If funcLiteral.Name.Value.Replace(" ", "") = Name Then
                Return funcLiteral
            End If
        Next

        Return Nothing
    End Function
End Class

Public Class Utils
    Public Shared Obj_Nothing As New Fox_Nothing()

    Public Shared Function ContainsKey(Dict, Key) As Boolean
        For Each k In Dict.Keys
            If Key = k Then
                Return True
            End If
        Next

        Return False
    End Function

    Public Shared Function GetKeyValue(Dict, Key)
        For Each keyPair In Dict
            If keyPair.Key = Key Then
                Return keyPair.Value
            End If
        Next

        Return Nothing
    End Function
End Class

Public Class StringUtils
    Public Shared Function Trim(str As String)
        Return str.Replace(vbCr, "").Replace(vbLf, "").Replace(vbCrLf, "").Replace(" ", "")
    End Function
End Class

Public Class FileSystemUtils

    Public Shared Function FindFile(folderPaths As List(Of String), fileName As String, fileExtension As String) As String
        For Each folderPath As String In folderPaths
            Dim filePath = $"{folderPath}\{fileName}.{fileExtension}".Replace("""", "")
            If Not File.Exists(filePath) Then Continue For

            Return filePath
        Next

        Return Nothing
    End Function

    Public Shared Function FindFolder(folderPaths As List(Of String), folderName As String) As String
        For Each folderPath As String In folderPaths
            Dim dirPath = $"{folderPath}\{folderName}".Replace("""", "")
            If Not Directory.Exists(dirPath) Then Continue For

            Return dirPath
        Next

        Return Nothing
    End Function
End Class

