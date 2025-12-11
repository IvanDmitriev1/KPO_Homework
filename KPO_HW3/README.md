# Prerequisites
- dotnet sdk 10.0
- docker

# How to deploy
- `dotnet publish /t:PublishContainer`
- `cd ./deploy`
- `docker compose up`

# Архитектура

Проект состоит из трёх микросервисов:

1. AppHost - aspire оркестрация
2. Api – отвечает за routing запросов.
3. File Analysis Service – отвечает за анализ на полагиат.
4. Files Storing Service – отвечает за хранение и выдачу файлов.

# Базы данных
Используется PostgreSql. \
Так же используется S3 хранилище - Minio.

- File Analysis Service – хранит информацию о заданиях на анализ и результаты анализа.
- Files Storing Service – хранит информацию о загруженных файлах и их метаданных.

# Curl-запросы:
Загрузка файла:
```bash
curl 'http://localhost:8080/works/upload?studentId=0e94e391-950d-4179-816c-9c078c4f87ec&assignmentId=0e94e391-950d-4179-816c-9c078c4f87ec' \
  --request POST \
  --header 'Content-Type: multipart/form-data' \
  --form 'file=@<FILE_NAME>.txt'
```

Получение результата анализа:
```bash
curl http://localhost:8080/works/<ID>/reports
```

Скачать загруженный файл:
```bash
curl http://localhost:8080/works/<ID>/content
```

WordCloud:
```bash
curl http://localhost:8080/works/<ID>/wordcloud
```