using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class CSVReader
{
    static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))"; //"가 뒤에 짝수개 등장하는 ,를 의미함
    static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";                   //모든 형태의 엔터를 의미함
    static char[] TRIM_CHARS = { '\"' };                                // 그냥 \"이거를 의미하는데 이게 왜 들어가지

    public static List<Dictionary<string, object>> Read(string file)    //이거 함수다 Read라는 이름에 file명을 매개변수로 받는
    {
        var list = new List<Dictionary<string, object>>();              //var은 리스트 선언에 쓰는 자료형이 맞는거같고
        TextAsset data = Resources.Load(file) as TextAsset;             //매개변수로 받은 파일명으로 리소스를 찾아서 data에 할당

        var lines = Regex.Split(data.text, LINE_SPLIT_RE);              //데이터의 텍스트를 줄단위로 나눈다

        if (lines.Length <= 1) return list;                             //lines배열의 원소가 1개거나 그이하면 바로 반환

        var header = Regex.Split(lines[0], SPLIT_RE);                   //라인의 첫줄을 ,를 기준으로 나눠 header에 저장
        for (var i = 1; i < lines.Length; i++)                          //라인의 길이만큼 1번줄부터 끝줄까지에 대해서 실행
        {

            var values = Regex.Split(lines[i], SPLIT_RE);               //lines를 나눠 values에 저장
            if (values.Length == 0 || values[0] == "") continue;        //values배열의 원소가 없거나, 첫 원소가 없을 경우 다음으로 넘어감

            var entry = new Dictionary<string, object>();               //딕셔너리가 맵이고 위에는 맵의 리스트인듯
            for (var j = 0; j < header.Length && j < values.Length; j++)//헤더와 밸류의 양쪽의 길이보다 작을 경우에만 실행
            {
                string value = values[j];
                value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", ""); 
                //value의 앞뒤에 있는 trim_chars를 모두 없에고 \\를 공백으로 대체
                object finalvalue = value;
                int n;
                float f;
                if (int.TryParse(value, out n))
                {
                    finalvalue = n;
                }
                else if (float.TryParse(value, out f))
                {
                    finalvalue = f;
                }//만들어진 finalvalue를 정수나 유리수로 변환 가능하면 int나 float의 형태로 저장
                entry[header[j]] = finalvalue;
                //그렇게 만들어진 finalvalue를 entry의 인덱스에 맞는 위치에 저장
            }
            list.Add(entry);
            //그렇게 하나의 entry가 만들어질때마다 entry(딕셔너리)의 리스트인 list에 저장
        }
        return list;    //리스트 반환
    }
}

/*
var
@
\"
Dictionary ? manager.battleMap
Resources가 resource폴더를 요구하는 이유
object
 */