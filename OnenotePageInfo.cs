using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneNoteDuplicatesRemover
{
    public class OnenotePageInfo
    {
        public string ParentSectionId; // 부모 섹션 ID
        public string ParentSectionName; // 부모 섹션 이름
        public string ParentSectionPath; // 부모 섹션 파일 위치
        public string PageName; // 페이지 명
        public string InnerTextHash; // 페이지 내용에 대한 Hash 값
    }
}
