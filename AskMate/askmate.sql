--
-- PostgreSQL database dump
--

SET statement_timeout = 0;
SET lock_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SET check_function_bodies = false;
SET client_min_messages = warning;



SET default_tablespace = '';

SET default_with_oids = false;


---
--- drop tables
---


DROP TABLE IF EXISTS questions;
DROP TABLE IF EXISTS answers;
DROP TABLE IF EXISTS users;


CREATE TABLE questions (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255),
    description VARCHAR(255),
    author VARCHAR(255),
    submission_time DATE
);

CREATE TABLE answers (
    id SERIAL PRIMARY KEY,
    message VARCHAR(255),
    question_id int NOT NULL,
    accepted boolean DEFAULT false,
    submission_time DATE
);

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(255),
    email VARCHAR(255),
    password VARCHAR(255),
    registration_time DATE
);

INSERT INTO questions VALUES ('1','C# definition', 'I would like to ask what is the definition of c#','Kenyik', NOW());
INSERT INTO questions VALUES ('2','JS definition', 'I would like to ask what is the definition of js','Kenyik', NOW());
INSERT INTO questions VALUES ('3','Python definition', 'I would like to ask what is the definition of python', 'NoName', NOW());

INSERT INTO answers VALUES ('1','Dummy c# definition answer', 1,false , NOW());
INSERT INTO answers VALUES ('2','Dummy c# definition answer 2', 1,false , NOW());
INSERT INTO answers VALUES ('3','Dummy js definition answer', 2,false , NOW());
INSERT INTO answers VALUES ('4','Dummy python definition answer', 3,false , NOW());

INSERT INTO users VALUES ('1','Kenyik', 'kenyik@codecool.hu','Incorrect', NOW());
