//https://stackoverflow.com/questions/70535929/how-to-determine-when-a-synced-bookmark-in-microsoft-edge-was-created
//var timeValue = '13285408186887489';
//var timeValue = '13228215968000000';
var timeValue = '13362347413406690';
var w=new Date(Date.UTC(1601,0,1) + timeValue / 1000);
WScript.Echo(w)