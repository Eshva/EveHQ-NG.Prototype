!macro customInstall
  DetailPrint "Register evehq-ng URI Handler"
  DeleteRegKey HKCR "evehq-ng"
  WriteRegStr HKCR "evehq-ng" "" "URL:evehq-ng"
  WriteRegStr HKCR "evehq-ng" "EveHQ NG SSO authentication Protocol" ""
  WriteRegStr HKCR "evehq-ng\DefaultIcon" "" "$INSTDIR\${APP_EXECUTABLE_FILENAME}"
  WriteRegStr HKCR "evehq-ng\shell" "" ""
  WriteRegStr HKCR "evehq-ng\shell\Open" "" ""
  WriteRegStr HKCR "evehq-ng\shell\Open\command" "" "$INSTDIR\${APP_EXECUTABLE_FILENAME} %1"
!macroend
