### IMG_TEST[图片测试]
![image-title](img/test.jpg) 
![image-title](img/test2.vsd#2)   <!-- convert the 2nd page( pages start at 1) of test2.vsd to jpeg, then insert it into document-->
![image-title](img/test3.vsd)     <!--convert the 1st page of test2.vsd to jpeg by default, then insert it into document -->

<!-- Notice: Prefix "IMG_" must be used when inserting images -->


### CL_LIST
<!-- Single column cell list, the following list will be insert to the column where MergedField is. A new row will be append automatically -->
- Item 1
- **Item 2**
- Item 3
<!-- List item must use "-" as prefix, "**" bold syntax is supported -->


### LOCATION
Somewhere

### TEST_FIELD
[](#LOCATION) is a test filed. It quote from ###LOCATION
[](#LIST[2]) quote the second item in ###LIST (the index starts at 1)

### FILE_IMPORT
[$IMPORT](../assets/src.docx)     <!-- Notice: keyword $IMPORT must be used when importing external MS Word document-->

### COUNT_MEMBER
<!-- Internal function,Counter. Code below count the value of ### MEMBER, split with '、' by default -->
$COUNT([](#MEMBER),'、')