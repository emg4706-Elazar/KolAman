

# קול אמ"ן

רקע : עקב מתקפת סייבר קרסה מערכת התרעה 'קול אמ"ן' שאחראית לקבלת התרעות תתספר מקורות , עיבודן, סיווגן, והעברתן לגורמי הפיקוד הרלוונטים בזמן אמת.

המשימה : להקים מערכת חלופית שתשחזר את היכולות המערכת הקודמת.



### רכיבי הפרויקט:
1. 'NotificationGate'
2. 'ClassificationComponent'
3. 'ControlSystem'


### תשתיות:
1. Kafka
2. Rabbit
3. Redis
4. Kibana
5. elasticsearch


### alert types:
```json
{
  "alert_id": str,
  "source": str,
  "title": str,
  "content": str,
  "priority": str,
  "classification": str,
  "lat": long,
  "lon": long,
  "timestamp": str,
  "status": str
}
```



## NotificationGate

1. Model 'Alert'
מייצג את תוכן ההודעה שעלולה להגיע לכן כל השדות יכולות להיות מחרוזת או null.
2. Service 'WatchWorker'
אחראי על סריקה של התיקייה alerts ולהתריע במקרה שמתקבל קובץ json שלם, במקרה שהתקבל קובץ json שלם הוא קורה ל AlertProcessor ונותן לו כפרמטר את הנתיב אל הקובץ.
3. Service 'AlertProcessor'
מקבל את הנתיב אל הקובץ json שהתקבל,
מפעיל את JsonReaderService עם הנתיב של הקובץ,
שהוא קורא את הקובץ וממיר אותו לרשימה של אובייקטים מסוג 'Alert' ומחזיר את הרשימה ל AlertProcessor
ה AlertProcessor עובר בלולאה על רשימת ה alerts,
מפעיל את ה producer עם ה alert של אותה איטרציה.
4. Service 'JsonReaderService'
מקבל נתיב מ alertProcessor,
קורא את הקובץ וממיר את זה לרשימה של אובייקטים של Alert,
או במקרה של כשלון ל null,
ומחזיר את התוצאה.



## ClassificationComponent
1. קובץ ה main אחראי על צריכת ההתרעות מ kafka,
ממיר את ה json ל dict
לאחר מכן הוא קורא לפונקציית get_region_with_geopandas ונותן לה 3 פרמטרים
1 נתיב אל קובץ של פוליגוני הפיקודים . 2. longitude. 3. latitude
ומקבל בחזרה את המחרוזת שמייצגת את האזור
לאחר מכן הוא קורא ל פונקציית publish ששולחת את ההודעת לתור של הפיקוד אותה גזרה.
2. קובץ ה classification
מכיל את פונקציית get_region_with_geopandas שמקבלת את הפרמטרים ומבצעת סיווג לפי איזור ומחזירה מחרוזת שמייצגת את האיזור אל קובץ ה main
3. קובץ publisher
מכיל פונקציה בשם publish יוצר חיבור ל Rabbit ומבצע שליחה של ההתרעה אל התור הייעודי לאותה גזרה.