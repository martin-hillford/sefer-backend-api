-- This script contains the full schema of database as of migrations 38
-- but rewritten to Postgresql. Please note, that this script is not intended 
-- to run every deployment, though technically it is possible

-- This table contains all the donations made via the website
CREATE TABLE IF NOT EXISTS public.donations
(
    id            VARCHAR(36)   NOT NULL CONSTRAINT pk_donations PRIMARY KEY,
    creation_date TIMESTAMP     NOT NULL,
    status        SMALLINT      NOT NULL,
    amount        INT           NOT NULL,
    payment_id    TEXT
);

-- The users table is one of the primary tables with information. Currently, this does not 
-- include any references to other table. 
-- TODO preferred_interface_language is candidate for improvement of the data structure
CREATE TABLE IF NOT EXISTS public.users
(
    id                           INT GENERATED ALWAYS AS IDENTITY   NOT NULL CONSTRAINT pk_users PRIMARY KEY,
    name                         VARCHAR(255)                       NOT NULL,
    role                         SMALLINT                           NOT NULL,
    gender                       SMALLINT                           NOT NULL,
    email                        VARCHAR(450)                       NOT NULL,
    year_of_birth                SMALLINT                           NOT NULL,
    info                         TEXT,
    password                     TEXT,
    password_salt                TEXT,
    subscription_date            TIMESTAMP                          NOT NULL,
    active                       BOOLEAN                            NOT NULL,
    approved                     BOOLEAN                            NOT NULL,
    blocked                      BOOLEAN                            NOT NULL,
    notification_preference      SMALLINT                           NOT NULL,
    imported                     BOOLEAN    DEFAULT FALSE           NOT NULL,
    preferred_interface_language VARCHAR(3) DEFAULT N'NL',
    two_factor_auth_enabled      BOOLEAN    DEFAULT FALSE           NOT NULL,
    two_factor_auth_secret_key   VARCHAR(64),
    prefer_spoken_courses        BOOLEAN    DEFAULT FALSE           NOT NULL,
    allow_impersonation          BOOLEAN    DEFAULT FALSE           NOT NULL,
    primary_site                 VARCHAR(255),
    primary_region               VARCHAR(255)
);

CREATE UNIQUE INDEX IF NOT EXISTS ix_users_email ON public.users (email);
CREATE INDEX IF NOT EXISTS ix_users_notification_preference on public.users (notification_preference);
CREATE INDEX IF NOT EXISTS ix_users_role on public.users (role);
CREATE INDEX IF NOT EXISTS ix_users_year_of_birth on public.users (year_of_birth, role);

CREATE OR REPLACE FUNCTION fn_tg_users_insert() RETURNS TRIGGER AS
$$
BEGIN
    INSERT INTO public.user_last_activity VALUES (NEW.id, clock_timestamp());
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER tg_users_insert
    AFTER INSERT ON public.users
    FOR EACH ROW
EXECUTE FUNCTION fn_tg_users_insert();

-- The table user_last_activity contains the timestamp of when the user with the give id
-- did have any activity. This table is present for performance reasons and is maintained
-- via triggers. This table breaks data integrity as no foreign key is present on the data
-- since this caused issues in ef core. The impact of this is minimal though since
-- the data is not critical
CREATE TABLE IF NOT EXISTS public.user_last_activity
(
    user_id        INT       NOT NULL CONSTRAINT pk_user_last_activity PRIMARY KEY,
    activity_date  TIMESTAMP NOT NULL
);

-- table push_notification_tokens contains the tokens used to send push notifications
-- and identifies devices of users
CREATE TABLE IF NOT EXISTS public.push_notification_tokens
(
    user_id INT          NOT NULL,
    token   VARCHAR(255) NOT NULL
);
CREATE INDEX IF NOT EXISTS ix_push_notification_tokens_user_id ON public.push_notification_tokens (user_id) INCLUDE (token);

-- table login log entries contains logging of logon events by the system.
-- this data is used for both determining user activity, but foremost to prevent
-- brute forcing password as it can act for rate limiting
CREATE TABLE IF NOT EXISTS public.login_log_entries
(
    id                INT GENERATED ALWAYS AS IDENTITY  NOT NULL CONSTRAINT pk_login_log_entries PRIMARY KEY,
    log_time          TIMESTAMP NOT NULL,
    path              TEXT,
    user_agent        TEXT,
    browser_token     TEXT,
    accepted_language TEXT,
    ip_address        TEXT,
    username          VARCHAR(450),
    result            SMALLINT                          NOT NULL,
    user_id           INT
);

CREATE INDEX IF NOT EXISTS ix_login_log_entries_log_time ON public.login_log_entries (log_time);
CREATE INDEX IF NOT EXISTS ix_login_log_entries_user_id ON public.login_log_entries (user_id) INCLUDE (log_time);
CREATE INDEX IF NOT EXISTS ix_login_log_entries_username ON public.login_log_entries (username, log_time);

CREATE OR REPLACE FUNCTION fn_tg_login_log_entries_insert() RETURNS TRIGGER AS
$$
BEGIN
    UPDATE public.user_last_activity SET activity_date = NEW.log_time
    WHERE public.user_last_activity.user_id = NEW.user_id;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER tg_login_log_entries_insert
    AFTER INSERT ON public.login_log_entries
    FOR EACH ROW
EXECUTE FUNCTION fn_tg_login_log_entries_insert();

-- This table hold specific settings for students.
-- Currently, only if the student has a personal mentor assigned
-- TODO: include foreign key reference to the user table
CREATE TABLE IF NOT EXISTS public.student_settings
(
    id                 INT GENERATED ALWAYS AS IDENTITY NOT NULL CONSTRAINT pk_student_settings PRIMARY KEY,
    student_id         INT                              NOT NULL,
    personal_mentor_id INT
);
CREATE UNIQUE INDEX IF NOT EXISTS ix_student_settings_student_id ON public.student_settings (student_id);

-- Table for our short url service
CREATE TABLE IF NOT EXISTS public.short_urls
(
    id          VARCHAR(24)     NOT NULL    CONSTRAINT pk_short_urls PRIMARY KEY,
    destination TEXT            NOT NULL,
    fallback    TEXT,
    expires     TIMESTAMP
);

-- Table for storing log information in the application for debugging
CREATE TABLE IF NOT EXISTS public.logs
(
    id            UUID          NOT NULL DEFAULT gen_random_uuid() CONSTRAINT pk_logs PRIMARY KEY,
    timestamp     TIMESTAMP     NOT NULL,
    log_level     VARCHAR(50)   NOT NULL,
    category_name VARCHAR(255),
    message       TEXT,
    stack_trace   TEXT,
    exception     TEXT
);

CREATE INDEX IF NOT EXISTS ix_logs_category_name ON public.logs (category_name ASC, timestamp DESC);
CREATE INDEX IF NOT EXISTS ix_logs_timestamp ON public.logs (timestamp);

-- Contains available rewards
CREATE TABLE IF NOT EXISTS public.rewards
(
    id            INT GENERATED ALWAYS AS IDENTITY  NOT NULL CONSTRAINT pk_rewards PRIMARY KEY,
    name          VARCHAR(255)                      NOT NULL,
    description   TEXT                              NOT NULL,
    minimal_grade FLOAT,
    type          SMALLINT                          NOT NULL,
    method        SMALLINT                          NOT NULL
);
CREATE INDEX IF NOT EXISTS ix_rewards_type ON public.rewards (type);


-- A table with the target for a user to met for certain rewards
-- NOTE: the original name of the 'number' column was 'order'
CREATE TABLE IF NOT EXISTS public.reward_targets
(
    id          INT GENERATED ALWAYS AS IDENTITY     NOT NULL    CONSTRAINT pk_reward_targets PRIMARY KEY,
    reward_id   INT                                  NOT NULL    CONSTRAINT fk_reward_targets_reward_id
                                                                 REFERENCES public.rewards ON DELETE CASCADE ,
    target      INT                                  NOT NULL,
    value       FLOAT,
    "order"     INT                                  NOT NULL,
    is_deleted  BOOLEAN                              NOT NULL
);
CREATE INDEX IF NOT EXISTS ix_reward_targets_reward ON public.reward_targets (reward_id);

-- Table with a series, which groups series together at a certain level
CREATE TABLE IF NOT EXISTS public.series
(
    id                INT GENERATED ALWAYS AS IDENTITY      NOT NULL CONSTRAINT pk_series PRIMARY KEY,
    creation_date     TIMESTAMP                             NOT NULL,
    modification_date TIMESTAMP,
    name              VARCHAR(255)                          NOT NULL,
    description       TEXT                                  NOT NULL,
    level             SMALLINT                              NOT NULL,
    is_public         BOOLEAN                               NOT NULL,
    sequence_id       INT DEFAULT 0                         NOT NULL
);

-- Table with basic course information
CREATE TABLE IF NOT EXISTS public.courses
(
    id                             INT GENERATED ALWAYS AS IDENTITY     NOT NULL CONSTRAINT pk_courses PRIMARY KEY,
    creation_date                  TIMESTAMP                            NOT NULL,
    modification_date              TIMESTAMP,
    name                           VARCHAR(255),
    permalink                      VARCHAR(450),
    description                    TEXT                                 NOT NULL,
    level                          SMALLINT                             NOT NULL,
    is_video_course                BOOLEAN                              NOT NULL,
    large_image                    TEXT,
    thumbnail_image                TEXT,
    show_on_homepage               BOOLEAN                              NOT NULL,
    private                        BOOLEAN                              NOT NULL,
    author                         VARCHAR(255),
    max_lesson_submissions_per_day INT,
    webshop_link                   TEXT,
    copyright                      TEXT,
    introduction_link              TEXT,
    header_image                   TEXT,
    citation                       VARCHAR(255),
    copyright_logo                 VARCHAR(255)
);
CREATE INDEX IF NOT EXISTS ix_courses_permalink on public.courses (permalink);

-- Define the relationship (many to many) between courses and series
CREATE TABLE IF NOT EXISTS public.series_courses
(
    id          INT GENERATED ALWAYS AS IDENTITY        NOT NULL    CONSTRAINT pk_series_courses PRIMARY KEY,
    course_id   INT                                     NOT NULL    CONSTRAINT fk_series_courses_course_id
                                                                    REFERENCES public.courses ON DELETE CASCADE,
    series_id   INT                                     NOT NULL    CONSTRAINT fk_series_courses_series_id
                                                                    REFERENCES public.series ON DELETE CASCADE,
    sequence_id INT                                     NOT NULL
);
CREATE INDEX IF NOT EXISTS ix_series_courses_course_id on public.series_courses (course_id);
CREATE INDEX IF NOT EXISTS ix_series_courses_series_id on public.series_courses (series_id);

-- General settings for the website
CREATE TABLE IF NOT EXISTS public.settings
(
    id                             INT GENERATED ALWAYS AS IDENTITY NOT NULL CONSTRAINT pk_settings PRIMARY KEY,
    relative_age_factor            FLOAT                            NOT NULL,
    same_mentor_days               SMALLINT                         NOT NULL,
    optimal_age_difference         SMALLINT                         NOT NULL,
    student_active_days            SMALLINT                         NOT NULL,
    student_reminder_days          SMALLINT                         NOT NULL,
    backup_mentor_id               INT                              NOT NULL,
    max_lesson_submissions_per_day SMALLINT
);

-- Containing the revisions for each course
CREATE TABLE IF NOT EXISTS public.course_revisions
(
    id                INT GENERATED ALWAYS AS IDENTITY  NOT NULL    CONSTRAINT pk_course_revisions PRIMARY KEY,
    creation_date     TIMESTAMP                         NOT NULL,
    modification_date TIMESTAMP,
    stage             SMALLINT                          NOT NULL,
    version           INT                               NOT NULL,
    predecessor_id    INT                                           CONSTRAINT fk_course_revisions_predecessor_id
                                                                    REFERENCES public.course_revisions ON DELETE SET NULL,
    allow_self_study  BOOLEAN                           NOT NULL,
    course_id         INT                               NOT NULL    CONSTRAINT fk_course_revisions_course_id
                                                                    REFERENCES public.Courses ON DELETE CASCADE,
    survey_id         INT
);

CREATE INDEX IF NOT EXISTS ix_course_revisions_course_id on public.course_revisions (course_id);
CREATE INDEX IF NOT EXISTS ix_course_revisions_predecessor_id on public.course_revisions (predecessor_id);

-- After each course there is the option to answer a survey
CREATE TABLE IF NOT EXISTS public.surveys
(
    id                        INT GENERATED ALWAYS AS IDENTITY  NOT NULL    CONSTRAINT pk_surveys PRIMARY KEY,
    creation_date             TIMESTAMP                         NOT NULL,
    modification_date         TIMESTAMP,
    course_revision_id        INT                                           CONSTRAINT fk_surveys_course_revision_id
                                                                            REFERENCES public.course_revisions ON DELETE CASCADE,
    enable_survey             BOOLEAN                           NOT NULL,
    enable_course_rating      BOOLEAN                           NOT NULL,
    enable_mentor_rating      BOOLEAN                           NOT NULL,
    enable_testimonial        BOOLEAN                           NOT NULL,
    enable_social_permissions BOOLEAN                           NOT NULL,
    predecessor_id            INT                                           CONSTRAINT fk_surveys_predecessor_id
                                                                            REFERENCES public.surveys ON DELETE SET NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS ix_surveys_course_revision_id ON public.surveys (course_revision_id);
CREATE INDEX IF NOT EXISTS ix_surveys_predecessor_id on public.surveys (predecessor_id);

-- After creating the surveys also a reference from the course revisions must be added 
ALTER TABLE IF EXISTS public.course_revisions DROP CONSTRAINT IF EXISTS fk_course_revisions_survey_id;
ALTER TABLE IF EXISTS public.course_revisions ADD CONSTRAINT fk_course_revisions_survey_id FOREIGN KEY (survey_id) REFERENCES public.surveys;

-- A table with yes/no question for the survey
CREATE TABLE IF NOT EXISTS public.survey_bool_questions
(
    id                     INT GENERATED ALWAYS AS IDENTITY NOT NULL    CONSTRAINT pk_survey_bool_questions PRIMARY KEY,
    creation_date          TIMESTAMP                        NOT NULL,
    modification_date      TIMESTAMP,                
    survey_id              INT                              NOT NULL    CONSTRAINT fk_survey_bool_questions_survey_id
                                                                        REFERENCES public.surveys ON DELETE CASCADE,
    sequence_id            INT                              NOT NULL,
    text                   TEXT,                
    number                 TEXT,                
    force_page_break       BOOLEAN                          NOT NULL,
    heading                TEXT,
    predecessor_id         INT                                           CONSTRAINT fk_survey_bool_questions_predecessor_id
                                                                         REFERENCES public.survey_bool_questions ON DELETE SET NULL,
    correct_answer_is_true BOOLEAN                          NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_survey_bool_questions_predecessor_id ON public.survey_bool_questions (predecessor_id);
CREATE INDEX IF NOT EXISTS ix_survey_bool_questions_survey_id ON public.survey_bool_questions (survey_id);

-- A table with multiple choice question for the survey
CREATE TABLE IF NOT EXISTS public.survey_multiple_choice_questions
(
    id                INT GENERATED ALWAYS AS IDENTITY  NOT NULL    CONSTRAINT pk_survey_multiple_choice_questions PRIMARY KEY,
    creation_date     TIMESTAMP                         NOT NULL,
    modification_date TIMESTAMP,
    survey_id         INT                               NOT NULL    CONSTRAINT fk_survey_multiple_choice_questions_survey_id
                                                                    REFERENCES public.surveys ON DELETE CASCADE,
    sequence_id       INT                               NOT NULL,
    text              TEXT,
    Number            TEXT,
    force_page_break  BOOLEAN                           NOT NULL,
    heading           TEXT,
    predecessor_id    INT                                           CONSTRAINT fk_survey_multiple_choice_questions_predecessor_id
                                                                    REFERENCES public.survey_multiple_choice_questions ON DELETE SET NULL, 
    IsMultiSelect     BOOLEAN                           NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_survey_multiple_choice_questions_predecessor_id ON public.survey_multiple_choice_questions (predecessor_id);
CREATE INDEX IF NOT EXISTS ix_survey_multiple_choice_questions_SurveyId ON public.survey_multiple_choice_questions (survey_id);

-- the choices for multiple choice question for the survey
CREATE TABLE IF NOT EXISTS public.survey_multiple_choice_question_choices
(
    id                INT GENERATED ALWAYS AS IDENTITY  NOT NULL    CONSTRAINT pk_survey_multiple_choice_question_choices PRIMARY KEY,
    answer            TEXT                              NOT NULL,
    is_correct_answer BOOLEAN                           NOT NULL,
    question_id       INT                               NOT NULL    CONSTRAINT fk_survey_multiple_choice_questions_question_id
                                                                    REFERENCES public.survey_multiple_choice_question_choices ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS id_survey_multiple_choice_question_choices_question_id ON public.survey_multiple_choice_question_choices (question_id);

-- open question within a survey
CREATE TABLE IF NOT EXISTS public.survey_open_questions
(
    id                INT GENERATED ALWAYS AS IDENTITY  NOT NULL    CONSTRAINT pk_survey_open_questions PRIMARY KEY,
    creation_date     TIMESTAMP                         NOT NULL,
    modification_date TIMESTAMP,             
    survey_id         INT                               NOT NULL    CONSTRAINT fk_survey_open_questions_survey_id REFERENCES public.surveys ON DELETE CASCADE,
    sequence_id       INT                               NOT NULL,
    text              TEXT,             
    number            TEXT,             
    force_page_break  BOOLEAN                           NOT NULL,
    heading           TEXT,             
    predecessor_id    INT                                           CONSTRAINT fk_survey_open_questions_predecessor_id
                                                                    REFERENCES public.survey_open_questions ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_survey_open_questions_predecessor_id ON public.survey_open_questions (predecessor_id);
CREATE INDEX IF NOT EXISTS ix_survey_open_questions ON public.survey_open_questions (survey_id);

-- contains backup keys for users that have enabled two-factor authentication
CREATE TABLE IF NOT EXISTS public.user_backup_keys
(
    id         INT GENERATED ALWAYS AS IDENTITY NOT NULL CONSTRAINT pk_user_backup_keys PRIMARY KEY,
    user_id    INT                              NOT NULL CONSTRAINT fk_user_backup_keys_user_id REFERENCES public.users ON DELETE CASCADE,
    backup_key VARCHAR(255)
);

CREATE INDEX IF NOT EXISTS ix_user_backup_keys_user_id on public.user_backup_keys (user_id);

-- Information about a student that has been added by a mentor
CREATE TABLE IF NOT EXISTS public.mentor_student_data
(
    id         INT GENERATED ALWAYS AS IDENTITY NOT NULL CONSTRAINT pk_MentorStudentData PRIMARY KEY,
    mentor_id  INT                              NOT NULL CONSTRAINT fk_MentorStudentData_MentorId REFERENCES public.users ON DELETE CASCADE,
    student_id INT                              NOT NULL CONSTRAINT fk_MentorStudentData_StudentId REFERENCES public.users ON DELETE CASCADE,
    remarks    TEXT
);

CREATE UNIQUE INDEX IF NOT EXISTS ix_MentorStudentId ON public.mentor_student_data (mentor_id, student_id);

-- A table that contains which mentor is teaching which course
CREATE TABLE IF NOT EXISTS public.mentor_courses
(
    id        INT GENERATED ALWAYS AS IDENTITY  NOT NULL    CONSTRAINT pk_mentor_courses PRIMARY KEY,
    course_id INT                               NOT NULL    CONSTRAINT fk_mentor_courses_course_id REFERENCES public.Courses ON DELETE CASCADE,
    mentor_id  INT                              NOT NULL    CONSTRAINT fk_mentor_courses_mentorId REFERENCES public.Users ON DELETE CASCADE,
                                                            CONSTRAINT UQ_MentorCourses UNIQUE (course_id, mentor_id)
);

CREATE INDEX IF NOT EXISTS ix_mentor_courses_course_id ON public.mentor_courses (course_id);
CREATE INDEX IF NOT EXISTS ix_mentor_courses_MentorId ON public.mentor_courses (mentor_id);

-- A table that contains specific settings for mentor
CREATE TABLE IF NOT EXISTS public.mentor_settings
(
    id                  INT GENERATED ALWAYS AS IDENTITY    NOT NULL    CONSTRAINT pk_mentor_settings PRIMARY KEY,
    mentor_id           INT                                 NOT NULL    CONSTRAINT fk_mentor_settings_Mentor_Id
                                                                        REFERENCES public.users ON DELETE CASCADE,
    supervisor_id       INT                                             CONSTRAINT fk_mentor_settings_supervisor_id
                                                                        REFERENCES public.users ON DELETE SET NULL,
    maximum_students    SMALLINT                            NOT NULL,
    preferred_students  SMALLINT                            NOT NULL,
    allow_overflow      BOOLEAN                             NOT NULL,
    is_personal_mentor  BOOLEAN                             NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS ix_mentor_settings_mentor_id ON public.mentor_settings (mentor_id);
CREATE INDEX IF NOT EXISTS ix_mentor_settings_supervisor_id ON public.mentor_settings (supervisor_id);

-- Awarded rewards for students
CREATE TABLE IF NOT EXISTS public.reward_grants
(
    id             INT GENERATED ALWAYS AS IDENTITY NOT NULL    CONSTRAINT pk_reward_grants PRIMARY KEY,
    date           TIMESTAMP                        NOT NULL,
    user_id        INT                              NOT NULL    CONSTRAINT fk_reward_grants_user_id
                                                                REFERENCES public.users ON DELETE CASCADE,
    target_reached INT                              NOT NULL,
    target_value   FLOAT,           
    target_id      INT                              NOT NULL    CONSTRAINT fk_reward_grants_target_id
                                                                REFERENCES public.reward_targets ON DELETE CASCADE,
    reward_id      INT                              NOT NULL    CONSTRAINT fk_reward_grants_reward_id
                                                                REFERENCES public.rewards ON DELETE CASCADE,
    code           TEXT
);

CREATE INDEX IF NOT EXISTS ix_reward_grants_RewardId ON public.reward_grants (reward_id);
CREATE INDEX IF NOT EXISTS ix_reward_grants_TargetId ON public.reward_grants (target_id);
CREATE INDEX IF NOT EXISTS ix_reward_grants_user_id ON public.reward_grants (user_id);

-- This tables contains to which regions mentors are assigned
CREATE TABLE IF NOT EXISTS public.mentor_regions
(
    id         INT GENERATED ALWAYS AS IDENTITY     NOT NULL    CONSTRAINT pk_mentor_regions PRIMARY KEY,
    mentor_id  INT                                  NOT NULL    CONSTRAINT fk_mentor_regions_mentor_id
                                                                REFERENCES public.users ON DELETE CASCADE,
    region_id  VARCHAR(50) NOT NULL
);

CREATE UNIQUE INDEX IF NOT EXISTS ix_mentor_regions on public.mentor_regions (mentor_id, region_id);

-- Contains which student is following which course
CREATE TABLE IF NOT EXISTS public.enrollments
(
    id                        INT GENERATED ALWAYS AS IDENTITY      NOT NULL    CONSTRAINT pk_enrollments PRIMARY KEY,
    creation_date             TIMESTAMP                             NOT NULL,
    modification_date         TIMESTAMP,         
    course_revision_id        INT                                   NOT NULL    CONSTRAINT fk_enrollments_course_revision_id
                                                                                REFERENCES public.course_revisions ON DELETE CASCADE,
    student_id                INT                                   NOT NULL    CONSTRAINT fk_enrollments_student_id
                                                                                REFERENCES public.users ON DELETE CASCADE,
    mentor_id                 INT                                               CONSTRAINT fk_enrollments_mentor_id
                                                                                REFERENCES public.users ON DELETE SET NULL,
    accountability_partner_id INT                                               CONSTRAINT fk_enrollments_accountability_partner_id
                                                                                REFERENCES public.users ON DELETE SET NULL,
    closure_date              TIMESTAMP,         
    is_course_completed       BOOLEAN                                   NOT NULL,
    survey_submitted          BOOLEAN                                   NOT NULL,
    allow_retake              BOOLEAN                                   NOT NULL,
    grade                     FLOAT,            
    imported                  BOOLEAN                                   NOT NULL,
    on_paper                  BOOLEAN                                   NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_enrollments_accountability_partner_id on public.enrollments (accountability_partner_id);
CREATE INDEX IF NOT EXISTS ix_enrollments_closure_date on public.enrollments (closure_date);
CREATE INDEX IF NOT EXISTS ix_enrollments_course_revision_id on public.enrollments (course_revision_id);
CREATE INDEX IF NOT EXISTS ix_enrollments_is_course_completed on public.enrollments (is_course_completed);
CREATE INDEX IF NOT EXISTS ix_enrollments_mentor_id on public.enrollments (mentor_id);
CREATE INDEX IF NOT EXISTS ix_enrollments_student_id on public.enrollments (student_id);

CREATE OR REPLACE FUNCTION fn_tg_enrollments_insert() RETURNS TRIGGER AS
$$
BEGIN
    UPDATE public.user_last_activity SET activity_date = clock_timestamp()
    WHERE user_id = NEW.student_id;
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER tg_enrollments_insert
    AFTER INSERT ON public.enrollments
    FOR EACH ROW
EXECUTE FUNCTION fn_tg_enrollments_insert();

-- defines the relationship between rewards and enrollments
CREATE TABLE IF NOT EXISTS public.reward_enrollments
(
    id            INT GENERATED ALWAYS AS IDENTITY NOT NULL CONSTRAINT pk_reward_enrollments PRIMARY KEY,
    enrollment_id INT NOT NULL CONSTRAINT fk_reward_enrollments_enrollment_id REFERENCES public.enrollments ON DELETE CASCADE,
    reward_id     INT NOT NULL CONSTRAINT fk_reward_enrollments_reward_id REFERENCES public.Rewards ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS ix_reward_enrollments_enrollment_id on public.reward_enrollments (enrollment_id);
CREATE INDEX IF NOT EXISTS ix_reward_enrollments_RewardId on public.reward_enrollments (reward_id);

-- students can rate the mentor after completing a course
CREATE TABLE IF NOT EXISTS public.mentor_ratings
(
    id            INT GENERATED ALWAYS AS IDENTITY  NOT NULL CONSTRAINT pk_mentor_ratings PRIMARY KEY,
    creation_date TIMESTAMP                         NOT NULL,
    mentor_id     INT                               NOT NULL CONSTRAINT fk_mentor_ratings_mentor_id REFERENCES public.users ON DELETE CASCADE,
    rating        SMALLINT                          NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_mentor_ratings_mentor_id on public.mentor_ratings (mentor_id);

-- students can rate the course after completing a course
CREATE TABLE IF NOT EXISTS public.course_ratings
(
    id            INT GENERATED ALWAYS AS IDENTITY  NOT NULL    CONSTRAINT pk_course_ratings PRIMARY KEY,
    creation_date TIMESTAMP                         NOT NULL,
    course_id     INT                               NOT NULL    CONSTRAINT fk_course_ratings_mentor_id
                                                                REFERENCES public.courses ON DELETE CASCADE,
    rating        SMALLINT                          NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_course_ratings_course_id on public.course_ratings (course_id);

-- A table that contains the results of the survey after the course
CREATE TABLE IF NOT EXISTS public.survey_results
(
    id                 INT GENERATED ALWAYS AS IDENTITY NOT NULL    CONSTRAINT pk_survey_results PRIMARY KEY,
    creation_date      TIMESTAMP                        NOT NULL,
    course_rating_id   INT                                          CONSTRAINT fk_survey_results_course_rating_id
                                                                    REFERENCES public.course_ratings ON DELETE SET NULL,
    mentor_rating_id   INT                                          CONSTRAINT fk_survey_results_mentor_rating_id
                                                                    REFERENCES public.mentor_ratings ON DELETE SET NULL,
    enrollment_id      INT                              NOT NULL    CONSTRAINT fk_survey_results_enrollment_id
                                                                    REFERENCES public.enrollments ON DELETE CASCADE,
    text               TEXT,
    social_permissions BOOLEAN                          NOT NULL,
    admin_processed    BOOLEAN                          NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_survey_results_course_rating_id on public.survey_results (course_rating_id);
CREATE INDEX IF NOT EXISTS ix_survey_results_enrollment_id on public.survey_results (enrollment_id);
CREATE INDEX IF NOT EXISTS ix_survey_results_mentor_rating_id on public.survey_results (mentor_rating_id);

-- Students can write a testimony after they have completed a course
CREATE TABLE IF NOT EXISTS public.testimonies
(
    id                INT GENERATED ALWAYS AS IDENTITY                       NOT NULL CONSTRAINT pk_testimonies PRIMARY KEY,
    creation_date     TIMESTAMP  WITH TIME ZONE    NOT NULL,
    modification_date TIMESTAMP,
    course_id         INT                          CONSTRAINT fk_testimonies_course_id REFERENCES public.courses ON DELETE SET NULL,
    student_id        INT                          CONSTRAINT fk_testimonies_student_id REFERENCES public.users ON DELETE SET NULL,
    content           TEXT                         NOT NULL,
    name              VARCHAR(255),
    survey_result_id  INT                          CONSTRAINT fk_testimonies_survey_result_id REFERENCES public.survey_results ON DELETE SET NULL ,
    is_anonymous      BOOLEAN                          NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_testimonies_course_id on public.testimonies (course_id);
CREATE INDEX IF NOT EXISTS ix_testimonies_student_id_course_id on public.testimonies (student_id, course_id);

-- special settings for admin users
CREATE TABLE IF NOT EXISTS public.admin_settings
(
    id              INT GENERATED ALWAYS AS IDENTITY  NOT NULL CONSTRAINT pk_admin_settings PRIMARY KEY,
    admin_id        INT     NOT NULL CONSTRAINT fk_admin_settings_admin_id REFERENCES public.users ON DELETE CASCADE,
    is_public_admin BOOLEAN     NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_admin_settings_AdminId on public.admin_settings (admin_id);

-- the lesson a student can follow
CREATE TABLE IF NOT EXISTS public.lessons
(
    id                 INT GENERATED ALWAYS AS IDENTITY                      NOT NULL     CONSTRAINT pk_lessons PRIMARY KEY,
    creation_date      TIMESTAMP    NOT NULL,
    modification_date  TIMESTAMP,
    sequence_id        INT                         NOT NULL,
    number             VARCHAR(50)                 NOT NULL,
    name               VARCHAR(50)                 NOT NULL,
    description        TEXT,
    course_revision_id INT                         NOT NULL     CONSTRAINT fk_lessons_course_revision_id
                                                                REFERENCES public.course_revisions ON DELETE CASCADE,
    predecessor_id     INT                                      CONSTRAINT fk_lessons_lessons_predecessor_id
                                                                REFERENCES public.lessons ON  DELETE  SET NULL ,
    read_before_start  TEXT,
    audio_reference_id UUID
);

CREATE INDEX IF NOT EXISTS ix_lessons_course_revision_id ON public.lessons (course_revision_id);
CREATE INDEX IF NOT EXISTS ix_lessons_predecessor_id ON public.lessons (predecessor_id);

-- Table with yes/no question in lessons
CREATE TABLE IF NOT EXISTS public.lesson_bool_questions
(
    id                     INT GENERATED ALWAYS AS IDENTITY                    NOT NULL   CONSTRAINT pk_lesson_bool_questions PRIMARY KEY,
    creation_date          TIMESTAMP  NOT NULL,
    modification_date      TIMESTAMP,
    lesson_id              INT                       NOT NULL   CONSTRAINT fk_lesson_bool_questions_lesson_id REFERENCES public.lessons ON DELETE CASCADE,
    sequence_id            INT                       NOT NULL,
    number                 TEXT,
    force_page_break       BOOLEAN                       NOT NULL,
    Heading                TEXT,
    predecessor_id         INT                                  CONSTRAINT fk_lesson_bool_questions_predecessor_id
                                                                REFERENCES public.lesson_bool_questions ON DELETE SET NULL,
    content                TEXT                      NOT NULL,
    correct_answer_is_true BOOLEAN                       NOT NULL,
    is_mark_down_content   BOOLEAN                       NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_lesson_bool_questions_lesson_id on public.lesson_bool_questions (lesson_id);
CREATE INDEX IF NOT EXISTS ix_lesson_bool_questions_predecessor_id on public.lesson_bool_questions (predecessor_id);

-- Media elements in lessons 
CREATE TABLE IF NOT EXISTS public.lesson_media_elements
(
    id                      INT GENERATED ALWAYS AS IDENTITY                      NOT NULL    CONSTRAINT pk_lesson_media_elements PRIMARY KEY,
    creation_date           TIMESTAMP    NOT NULL,
    modification_date       TIMESTAMP,
    lesson_id               INT                         NOT NULL    CONSTRAINT fk_lesson_media_elements_lesson_id
                                                                    REFERENCES public.lessons ON DELETE CASCADE,
    sequence_id             INT                         NOT NULL,
    number                  TEXT,
    force_page_break        BOOLEAN                         NOT NULL,
    heading                 TEXT,
    predecessor_id          INT                                     CONSTRAINT fk_lesson_media_elements_predecessor_id
                                                                    REFERENCES public.lesson_media_elements ON DELETE SET NULL,
    content                 TEXT,
    url                     TEXT                        NOT NULL,
    type                    SMALLINT                    NOT NULL,
    is_mark_down_content    BOOLEAN                         NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_lesson_media_elements_lesson_id ON public.lesson_media_elements (lesson_id);
CREATE INDEX IF NOT EXISTS ix_lesson_media_elements_predecessor_id ON public.lesson_media_elements (predecessor_id);

-- multiple choice question in lessons
CREATE TABLE IF NOT EXISTS public.lesson_multiple_choice_questions
(
    id                   INT GENERATED ALWAYS AS IDENTITY                     NOT NULL    CONSTRAINT pk_lesson_multiple_choice_questions  PRIMARY KEY,
    creation_date        TIMESTAMP   NOT NULL,
    modification_date    TIMESTAMP,
    lesson_id            INT                        NOT NULL    CONSTRAINT fk_lesson_multiple_choice_questions_lesson_id
                                                                REFERENCES public.lessons ON DELETE CASCADE,
    sequence_id          INT                        NOT NULL,
    number               TEXT,
    force_page_break     BOOLEAN                        NOT NULL,
    heading              TEXT,
    predecessor_id       INT                                    CONSTRAINT fk_lesson_multiple_choice_questions_predecessor_id
                                                                REFERENCES public.lesson_multiple_choice_questions ON DELETE SET NULL,
    content              TEXT                       NOT NULL,
    is_multi_select      BOOLEAN                        NOT NULL,
    is_mark_down_content BOOLEAN                        NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_lesson_multiple_choice_questions_lesson_id ON public.lesson_multiple_choice_questions (lesson_id);
CREATE INDEX IF NOT EXISTS ix_lesson_multiple_choice_questions_predecessor_id ON public.lesson_multiple_choice_questions (predecessor_id);

-- choices for multiple choice question in lessons
CREATE TABLE IF NOT EXISTS public.lesson_multiple_choice_question_choices
(
    Id                  INT GENERATED ALWAYS AS IDENTITY           NOT NULL  CONSTRAINT pk_lesson_multiple_choice_question_choices PRIMARY KEY,
    answer              TEXT          NOT NULL,
    is_correct_answer   BOOLEAN           NOT NULL,
    question_id         INT           NOT NULL  CONSTRAINT fk_lesson_multiple_choice_question_choices_question_id
                                                REFERENCES public.lesson_multiple_choice_questions ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS ix_lesson_multiple_choice_question_choice_question_id ON public.lesson_multiple_choice_question_choices(question_id);

-- Open questions for lessons
CREATE TABLE IF NOT EXISTS public.lesson_open_questions
(
    id                   INT GENERATED ALWAYS AS IDENTITY                         NOT NULL    CONSTRAINT pk_lesson_open_questions PRIMARY KEY,
    creation_date        TIMESTAMP       NOT NULL,
    modification_date    TIMESTAMP,
    lesson_id            INT                            NOT NULL    CONSTRAINT fk_lesson_open_questions_lesson_id
                                                                    REFERENCES public.lessons ON DELETE CASCADE,
    sequence_id          INT                            NOT NULL,
    Number               TEXT,
    force_page_break     BOOLEAN                            NOT NULL,
    heading              TEXT,
    predecessor_id       INT                                        CONSTRAINT fk_lesson_open_questions__predecessor_id
                                                                    REFERENCES public.lesson_open_questions ON DELETE SET NULL,
    content              TEXT                           NOT NULL,
    is_mark_down_content BOOLEAN                            NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_lesson_open_questions_lesson_id ON public.lesson_open_questions (lesson_id);
CREATE INDEX IF NOT EXISTS ix_lesson_open_questions_predecessor_id ON public.lesson_open_questions (predecessor_id);

-- text elements in lessons
CREATE TABLE IF NOT EXISTS public.lesson_text_elements
(
    id                   INT GENERATED ALWAYS AS IDENTITY                          NOT NULL      CONSTRAINT pk_lesson_text_elements PRIMARY KEY,
    creation_date        TIMESTAMP     NOT NULL,
    modification_date    TIMESTAMP,
    lesson_id            INT                          NOT NULL      CONSTRAINT fk_lesson_text_elements_lesson_id
                                                                    REFERENCES public.lessons ON DELETE CASCADE,
    sequence_id          INT                          NOT NULL,
    Number               TEXT,
    force_page_break     BOOLEAN                          NOT NULL,
    heading              TEXT,
    predecessor_id       INT                                        CONSTRAINT fk_lesson_text_elements_predecessor_id
                                                                    REFERENCES public.lesson_text_elements ON DELETE SET NULL,
    Content              TEXT                         NOT NULL,
    Type                 SMALLINT                     NOT NULL,
    is_mark_down_content BOOLEAN                          NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_lesson_text_elements_lesson_id ON public.lesson_text_elements (lesson_id);
CREATE INDEX IF NOT EXISTS ix_lesson_text_elements_predecessor_id ON public.lesson_text_elements (predecessor_id);

-- Table to track of the migrations
CREATE TABLE IF NOT EXISTS public.__migrations
(
    Number      INT          NOT NULL   CONSTRAINT pk__Migrations PRIMARY KEY,
    Name        VARCHAR(255) NOT NULL,
    Date        TIMESTAMP    NOT NULL
);

-- This table contains details of submitted lessons by students
CREATE TABLE IF NOT EXISTS public.lesson_submissions
(
    id                     INT GENERATED ALWAYS AS IDENTITY                       NOT NULL    CONSTRAINT pk_lesson_submissions PRIMARY KEY,
    creation_date          TIMESTAMP     NOT NULL,
    modification_date      TIMESTAMP,
    enrollment_id          INT                           NOT NULL   CONSTRAINT fk_lesson_submissions_enrollments_enrollment_id
                                                                    REFERENCES public.enrollments ON DELETE CASCADE,
    lesson_id              INT           NOT NULL                   CONSTRAINT fk_lesson_submissions_lesson_id
                                                                    REFERENCES public.lessons ON DELETE CASCADE,
    grade                   FLOAT,
    is_final                BOOLEAN                         NOT NULL,
    results_student_visible BOOLEAN                         NOT NULL,
    review_date             TIMESTAMP,
    imported                BOOLEAN                         NOT NULL,
    submission_date         TIMESTAMP,
    current_page            INT DEFAULT 1               NOT NULL,
    audio_track             INT DEFAULT 0               NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_lesson_submissions_enrollment_id ON public.lesson_submissions (enrollment_id);

CREATE INDEX IF NOT EXISTS ix_lesson_submissions_is_final
       ON public.lesson_submissions (is_final, results_student_visible, submission_date)
       INCLUDE (creation_date, current_page, enrollment_id, grade, imported, lesson_id, modification_date, review_date);

CREATE INDEX IF NOT EXISTS ix_lesson_submissions_lesson_id ON public.lesson_submissions (lesson_id);

-- Create the insert trigger that updated user_last_activity when a submission is inserted
CREATE OR REPLACE FUNCTION fn_tg_lesson_submissions_insert() RETURNS TRIGGER AS
$$
BEGIN
    UPDATE public.user_last_activity SET activity_date = clock_timestamp()
    WHERE user_id IN
    (
        SELECT student_id FROM enrollments
        WHERE enrollments.id = NEW.enrollment_id
    );
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER tg_lesson_submissions_insert
    AFTER INSERT ON public.lesson_submissions
    FOR EACH ROW
EXECUTE FUNCTION fn_tg_lesson_submissions_insert();

-- Create the update trigger that updated user_last_activity for students when a submission is updated
CREATE OR REPLACE FUNCTION fn_tg_lesson_submissions_student_update() RETURNS TRIGGER AS
$$
BEGIN
    UPDATE public.user_last_activity SET activity_date = clock_timestamp()
    WHERE user_id IN
          (
                SELECT e.student_id FROM NEW
                JOIN enrollments AS e ON e.id = NEW.enrollment_id
                WHERE NEW.results_student_visible = FALSE
          );
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER tg_lesson_submissions_student_update
    AFTER UPDATE ON public.lesson_submissions
    FOR EACH ROW
EXECUTE FUNCTION fn_tg_lesson_submissions_student_update();

-- Create the update trigger that updated user_last_activity for mentors when a submission is updated
CREATE OR REPLACE FUNCTION fn_tg_lesson_submissions_mentor_update() RETURNS TRIGGER AS
$$
BEGIN
    UPDATE public.user_last_activity SET activity_date = clock_timestamp()
    WHERE user_id IN
          (
                SELECT e.mentor_id FROM NEW
                JOIN enrollments AS e ON e.id = NEW.enrollment_id
                WHERE e.mentor_id IS NOT NULL AND NEW.results_student_visible = TRUE
          );
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER tg_lesson_submissions_mentor_update
    AFTER UPDATE ON public.lesson_submissions
    FOR EACH ROW
EXECUTE FUNCTION fn_tg_lesson_submissions_mentor_update();

-- Table with answers the student has given on the question of the lessons
CREATE TABLE IF NOT EXISTS public.answers
(
    id                  INT GENERATED ALWAYS AS IDENTITY                      NOT NULL    CONSTRAINT pk_answers   PRIMARY KEY,
    creation_date       TIMESTAMP    NOT NULL,
    modification_date   TIMESTAMP,
    question_id         INT                         NOT NULL,
    submission_id       INT                         NOT NULL    CONSTRAINT fk_answers_submission_id
                                                                REFERENCES public.lesson_submissions
                                                                ON DELETE CASCADE,
    text_answer         TEXT,
    question_type       SMALLINT  NOT NULL,
    is_correct_answer   BOOLEAN,
    mentor_review       TEXT
);

CREATE INDEX IF NOT EXISTS ix_answers_question_id ON public.answers (question_id);
CREATE INDEX IF NOT EXISTS ix_answers_submission_id ON public.answers (submission_id);

-- A table with logging api calls, is used for performance measurement
-- with some additions it can be used for rate limiting as well
CREATE TABLE IF NOT EXISTS public.api_request_Log_entries
(
    id                  BIGINT GENERATED ALWAYS AS IDENTITY               NOT NULL,
    log_time            TIMESTAMP   NOT NULL,
    path                VARCHAR(450),
    user_agent          TEXT,
    browser_token       TEXT,
    accepted_language   TEXT,
    processing_time     BIGINT                      NOT NULL,
    method              TEXT,
    do_not_track        BOOLEAN                         NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_api_request_Log_entries_log_time ON public.api_request_Log_entries (log_time);
CREATE INDEX IF NOT EXISTS ix_api_request_log_entries_path ON public.api_request_Log_entries (path);
CREATE INDEX IF NOT EXISTS ix_api_request_Log_entries_processing_time ON public.api_request_Log_entries (processing_time, log_time, path);

-- a table with blogs
CREATE TABLE IF NOT EXISTS public.blogs
(
    id                INT GENERATED ALWAYS AS IDENTITY                        NOT NULL CONSTRAINT pk_blogs PRIMARY KEY,
    creation_date     TIMESTAMP      NOT NULL,
    modification_date TIMESTAMP,
    is_published      BOOLEAN                           NOT NULL,
    publication_date  TIMESTAMP,
    name              VARCHAR(255)                  NOT NULL,
    permalink         VARCHAR(255),
    author_id         INT                           NOT NULL CONSTRAINT fk_blogs_author_id
                                                    REFERENCES public.users ON DELETE CASCADE,
    content           TEXT NOT NULL,
    is_html_content   BOOLEAN DEFAULT TRUE NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_blogs_author_id on public.blogs (author_id);
CREATE INDEX IF NOT EXISTS ix_blogs_permalink on public.blogs (permalink);
CREATE INDEX IF NOT EXISTS ix_blogs_permalink_is_published on public.blogs (permalink, is_published);
CREATE INDEX IF NOT EXISTS ix_blogs_publication_date_is_published on public.blogs (publication_date DESC, is_published ASC);

-- Channels for users to chat in
CREATE TABLE IF NOT EXISTS public.chat_channels
(
    id              INT GENERATED ALWAYS AS IDENTITY                      NOT NULL   CONSTRAINT pk_chat_channels PRIMARY KEY,
    creation_date   TIMESTAMP    NOT NULL,
    type            SMALLINT  NOT NULL,
    name            TEXT
);

-- Table with all the messages of user
CREATE TABLE IF NOT EXISTS public.chat_messages
(
    id                   BIGINT GENERATED ALWAYS AS IDENTITY                  NOT NULL   CONSTRAINT pk_chat_messages PRIMARY KEY,
    channel_id           INT                         NOT NULL   CONSTRAINT fk_chat_messages_chat_channel_id
                                                                REFERENCES public.chat_channels ON DELETE CASCADE,
    sender_id             INT                        NOT NULL   CONSTRAINT fk_chat_messages_sender_id
                                                                REFERENCES public.users ON DELETE CASCADE,
    sender_date           TIMESTAMP   NOT NULL,
    type                  SMALLINT                   NOT NULL,
    reference_id          INT,
    is_deleted            BOOLEAN                        NOT NULL,
    is_available          BOOLEAN                        NOT NULL,
    content_string        TEXT,
    quoted_message_string TEXT,
    quoted_message_id     BIGINT                             CONSTRAINT fk_chat_messages_chat_quoted_message_id 
                                                                REFERENCES public.chat_messages ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS ix_chat_messages_content_string ON public.chat_messages USING GIN ((to_tsvector('dutch', content_string)));
CREATE INDEX IF NOT EXISTS ix_chat_messages_channel_id ON public.chat_messages(channel_id, is_available);
CREATE INDEX IF NOT EXISTS ix_chat_messages_quoted_message_id ON public.chat_messages (quoted_message_id);
CREATE INDEX IF NOT EXISTS ix_chat_messages_sender_date ON public.chat_messages (sender_date);
CREATE INDEX IF NOT EXISTS ix_chat_messages_sender_id ON public.chat_messages (sender_id, channel_id) INCLUDE (sender_date);

-- Create the update trigger that updated user_last_activity for a user sends a message
CREATE OR REPLACE FUNCTION fn_tg_chat_messages_insert() RETURNS TRIGGER AS
$$
BEGIN
    UPDATE public.user_last_activity SET activity_date = clock_timestamp()
    WHERE user_id = NEW.sender_id AND NEW.type = 2;
END;
$$ LANGUAGE plpgsql;

CREATE OR REPLACE TRIGGER tg_chat_messages_update
    AFTER UPDATE ON public.chat_messages
    FOR EACH ROW
EXECUTE FUNCTION fn_tg_chat_messages_insert();

-- a table to keep track if user has read their messages
CREATE TABLE IF NOT EXISTS public.chat_channel_messages
(
    id          BIGINT GENERATED ALWAYS AS IDENTITY                   NOT NULL        CONSTRAINT pk_chat_channel_messages   PRIMARY KEY,
    receiver_id INT                         NOT NULL        CONSTRAINT fk_chat_channel_messages_receiver_id
                                                            REFERENCES public.users ON DELETE CASCADE,
    message_id  BIGINT                  NOT NULL        CONSTRAINT fk_chat_channel_messages_chat_messages_message_id
                                                            REFERENCES public.chat_messages ON DELETE CASCADE,
    read_date   TIMESTAMP,
    is_marked   BOOLEAN                         NOT NULL,
    is_notified BOOLEAN                         NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_chat_channel_messages_is_notified ON public.chat_channel_messages (is_notified);
CREATE INDEX IF NOT EXISTS ix_chat_channel_messages_message_id ON public.chat_channel_messages (message_id);
CREATE INDEX IF NOT EXISTS ix_chat_channel_messages_read_date ON public.chat_channel_messages (read_date, is_notified);
CREATE INDEX IF NOT EXISTS ix_chat_channel_messages_receiver_id ON public.chat_channel_messages (receiver_id, read_date, is_notified);
CREATE UNIQUE INDEX IF NOT EXISTS ix_chat_channel_messages_receiver_id_message_id ON public.chat_channel_messages (receiver_id, message_id);

-- A table to keep track which users is in which channel
CREATE TABLE IF NOT EXISTS public.chat_channel_receivers
(
    id                  INT GENERATED ALWAYS AS IDENTITY                         NOT NULL    CONSTRAINT pk_chat_channel_receivers PRIMARY KEY,
    creation_date       TIMESTAMP    NOT NULL,
    modification_date   TIMESTAMP,
    user_id             INT                         NOT NULL    CONSTRAINT fk_chat_channel_receivers_user_id
                                                                REFERENCES public.Users ON DELETE CASCADE,
    channel_id          INT                         NOT NULL    CONSTRAINT fk_chat_channel_receivers_channel_id
                                                                REFERENCES public.chat_channels ON DELETE CASCADE,
    has_post_rights     BOOLEAN                         NOT NULL,
    archived            BOOLEAN                         NOT NULL,
    deleted             BOOLEAN                         NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_chat_channel_receivers_channel_id ON public.chat_channel_receivers (channel_id);
CREATE INDEX IF NOT EXISTS ix_chat_channel_receivers_user_id ON public.chat_channel_receivers (user_id, archived, deleted) INCLUDE (channel_id);
CREATE UNIQUE INDEX IF NOT EXISTS ix_chat_channel_receivers_channel_id_user_id ON public.chat_channel_receivers (channel_id, user_id);

-- A table for analytics
CREATE TABLE IF NOT EXISTS public.client_page_request_log_entries
(
    id                  BIGINT GENERATED ALWAYS AS IDENTITY NOT NULL CONSTRAINT pk_client_page_request_log_entries PRIMARY KEY,
    log_time            TIMESTAMP                           NOT NULL,
    Path                VARCHAR(450),
    user_agent          TEXT,
    browser_token       VARCHAR(80),
    accepted_language   TEXT,
    screen_height       INT                                 NOT NULL,
    screen_width        INT                                 NOT NULL,
    do_not_track        BOOLEAN                             NOT NULL,
    is_unique_visit     BOOLEAN DEFAULT TRUE                NOT NULL,
    region              VARCHAR(50) DEFAULT 'nl'            NOT NULL,
    cmp                 VARCHAR(255),
    ip_address          VARCHAR(52),
    geo_ip_info_id      UUID,
    browser_class       VARCHAR(30) DEFAULT 'Unknown',
    operating_system    VARCHAR(30) DEFAULT 'Unknown',
    is_bot              BOOLEAN DEFAULT FALSE,
    site                VARCHAR(255)
);

CREATE INDEX IF NOT EXISTS ix_client_page_request_log_entries_browser_class ON public.client_page_request_log_entries (browser_class);
CREATE INDEX IF NOT EXISTS ix_client_page_request_log_entries_browser_token ON public.client_page_request_log_entries (browser_token);
CREATE INDEX IF NOT EXISTS ix_client_page_request_log_entries_is_bot ON public.client_page_request_log_entries (is_bot);
CREATE INDEX IF NOT EXISTS ix_client_page_request_log_entries_log_time ON public.client_page_request_log_entries (log_time) INCLUDE (operating_system, browser_class, is_bot);
CREATE INDEX IF NOT EXISTS ix_client_page_request_log_entries_operating_system ON public.client_page_request_log_entries (operating_system);
CREATE INDEX IF NOT EXISTS ix_client_page_request_log_entries_Path ON public.client_page_request_log_entries (Path, is_unique_visit) INCLUDE (log_time);
CREATE INDEX IF NOT EXISTS ix_client_page_request_log_entries_region ON public.client_page_request_log_entries (region);
CREATE INDEX IF NOT EXISTS ix_client_page_request_log_entries_Resolution ON public.client_page_request_log_entries (screen_width);
CREATE INDEX IF NOT EXISTS ix_client_page_request_log_entries_site ON public.client_page_request_log_entries (site);

-- Table for general content pages (like the privacy information page)
CREATE TABLE IF NOT EXISTS public.content_pages
(
    id                INT GENERATED ALWAYS AS IDENTITY                       NOT NULL CONSTRAINT pk_content_pages  PRIMARY KEY,
    creation_date     TIMESTAMP      NOT NULL,
    modification_date TIMESTAMP,
    type              SMALLINT                      NOT NULL,
    sequence_id       INT,
    is_published      BOOLEAN                           NOT NULL,
    name              VARCHAR(255)                  NOT NULL,
    permalink         VARCHAR(255),
    content           TEXT                          NOT NULL,
    is_html_content   BOOLEAN DEFAULT TRUE        NOT NULL
);

-- Table for site specific content pages
CREATE TABLE IF NOT EXISTS public.site_specific_content_pages
(
    id                  INT GENERATED ALWAYS AS IDENTITY                      NOT NULL    CONSTRAINT pk_site_specific_content_pages PRIMARY KEY,
    site                VARCHAR(255)                NOT NULL,
    content_page_id     INT                                     CONSTRAINT fk_content_pages
                                                                REFERENCES public.content_pages ON DELETE CASCADE,
    creation_date       TIMESTAMP    NOT NULL,
    modification_date   TIMESTAMP,
    content             TEXT                        NOT NULL,
    is_published        BOOLEAN                         NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_content_pages_permalink ON public.content_pages (permalink);
CREATE INDEX IF NOT EXISTS ix_content_pages_permalink_is_published ON public.content_pages (permalink ASC, is_published DESC);
CREATE INDEX IF NOT EXISTS ix_content_pages_type_is_published ON public.content_pages (type ASC, is_published DESC);


-- prerequisites a student must meet before starting a certain course
CREATE TABLE IF NOT EXISTS public.course_prerequisites
(
    id                 INT GENERATED ALWAYS AS IDENTITY   NOT NULL   CONSTRAINT pk_course_prerequisites  PRIMARY KEY,
    course_id          INT      NOT NULL    CONSTRAINT fk_course_prerequisites_Courses_course_id
                                            REFERENCES public.Courses ON DELETE CASCADE,
    required_course_id INT      NOT NULL    CONSTRAINT fk_course_prerequisites_Courses_required_course_id
                                            REFERENCES public.Courses ON DELETE CASCADE 
);

CREATE INDEX IF NOT EXISTS ix_course_prerequisites_course_id ON public.course_prerequisites (course_id);
CREATE INDEX IF NOT EXISTS ix_course_prerequisites_required_course_id ON public.course_prerequisites (required_course_id);

-- not only are courses organised in series but also in curricula
CREATE TABLE IF NOT EXISTS public.curricula
(
    id                INT GENERATED ALWAYS AS IDENTITY                        NOT NULL CONSTRAINT pk_curricula PRIMARY KEY,
    creation_date     TIMESTAMP      NOT NULL,
    modification_date TIMESTAMP,
    name              VARCHAR(255)                  NOT NULL,
    permalink         VARCHAR(255),
    description       TEXT                          NOT NULL,
    level             SMALLINT                      NOT NULL
);

-- ip address lookup information for analytics
CREATE TABLE IF NOT EXISTS public.ip_address_lookups
(
    id              UUID                        NOT NULL CONSTRAINT pk_ip_address_lookups PRIMARY KEY,
    ip_address      VARCHAR(52)                 NOT NULL,
    date            TIMESTAMP    NOT NULL,
    country_name    VARCHAR(255),
    country_code    VARCHAR(10),
    city            VARCHAR(255),
    region          VARCHAR(255),
    region_code     VARCHAR(255),
    continent       VARCHAR(50),
    latitude        FLOAT,
    longitude       FLOAT
);

CREATE INDEX IF NOT EXISTS ix_ip_address_lookups_city_grouping ON public.ip_address_lookups (date, country_code, city);

-- the revisions of the curricula
CREATE TABLE IF NOT EXISTS public.curriculum_revisions
(
    id                INT GENERATED ALWAYS AS IDENTITY                        NOT NULL CONSTRAINT pk_curriculum_revisions PRIMARY KEY,
    creation_date     TIMESTAMP      NOT NULL,
    modification_date TIMESTAMP,
    stage             SMALLINT                      NOT NULL,
    version           INT                           NOT NULL,
    predecessor_id    INT                           CONSTRAINT fk_curriculum_revisions_predecessor_id
                                                    REFERENCES public.curriculum_revisions ON DELETE SET NULL,
    curriculum_id     INT       NOT NULL            CONSTRAINT fk_curriculum_revisions_curriculum_id
                                                    REFERENCES public.curricula ON DELETE CASCADE,
    years             SMALLINT   NOT NULL
);

CREATE INDEX IF NOT EXISTS ix_curriculum_revisions_curriculum_id ON public.curriculum_revisions (curriculum_id);
CREATE INDEX IF NOT EXISTS ix_curriculum_revisions_predecessor_id ON public.curriculum_revisions (predecessor_id);

-- the blocks in the curricula
CREATE TABLE IF NOT EXISTS public.curriculum_blocks
(
    id                      INT GENERATED ALWAYS AS IDENTITY                      NOT NULL    CONSTRAINT pk_curriculum_blocks PRIMARY KEY,
    creation_date           TIMESTAMP    NOT NULL,
    modification_date       TIMESTAMP,
    curriculum_revision_id  INT                         NOT NULL    CONSTRAINT fk_curriculum_blocks_curriculum_revision_id
                                                                    REFERENCES public.curriculum_revisions ON DELETE CASCADE,
    predecessor_id          INT                                     CONSTRAINT fk_curriculum_blocks_predecessor_id
                                                                    REFERENCES public.curriculum_blocks ON DELETE SET NULL,
    year                    SMALLINT      NOT NULL,
    sequence_id             INT           NOT NULL,
    name                    VARCHAR(255) NOT NULL,
    description             TEXT
);

-- the course in the blocks
CREATE TABLE IF NOT EXISTS public.curriculum_block_courses
(
    id          INT GENERATED ALWAYS AS IDENTITY  NOT NULL    CONSTRAINT pk_curriculum_block_courses PRIMARY KEY,
    sequence_id INT     NOT NULL,
    course_id   INT     NOT NULL    CONSTRAINT fk_curriculum_block_courses_courses_course_id
                                    REFERENCES public.courses ON DELETE CASCADE,
    block_id    INT     NOT NULL    CONSTRAINT fk_curriculum_block_courses_curriculum_blocks_block_id
                                    REFERENCES public.curriculum_blocks ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS ix_curriculum_block_courses_block_id ON public.curriculum_block_courses (block_id);
CREATE INDEX IF NOT EXISTS ix_curriculum_block_courses_course_id ON public.curriculum_block_courses (course_id);
CREATE INDEX IF NOT EXISTS ix_curriculum_blocks_curriculum_revision_id ON public.curriculum_blocks (curriculum_revision_id);
CREATE INDEX IF NOT EXISTS ix_curriculum_blocks_predecessor_id ON public.curriculum_blocks (predecessor_id);

-- a table for the user-settings
CREATE TABLE IF NOT EXISTS public.user_settings
(
    id          INT GENERATED ALWAYS AS IDENTITY  NOT NULL  CONSTRAINT pk_user_settings PRIMARY KEY,
    user_id     INT                               NOT NULL  CONSTRAINT fk_user_settings_user_id
                                                            REFERENCES public.users ON DELETE CASCADE,
    key_name    VARCHAR(255)                      NOT NULL,
    value       VARCHAR(255)                      NOT NULL
);
CREATE INDEX IF NOT EXISTS ix_user_settings_user_id ON public.user_settings(user_id);

-- A table with template for e-mails and pdfs
CREATE TABLE IF NOT EXISTS public.templates
(
    id INT GENERATED ALWAYS AS IDENTITY NOT NULL CONSTRAINT pk_templates PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    title VARCHAR(255),
    layoutName VARCHAR(255),
    language VARCHAR(3) NOT NULL,
    content TEXT NOT NULL
);
CREATE INDEX IF NOT EXISTS ix_template_names_language ON public.templates (name,language);

-- A table with localization for notifications
CREATE TABLE IF NOT EXISTS public.notification_localizations
(
    id INT GENERATED ALWAYS AS IDENTITY NOT NULL CONSTRAINT pk_notification_localizations PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    language VARCHAR(3) NOT NULL,
    content TEXT NOT NULL
);
CREATE INDEX IF NOT EXISTS ix_notification_localizations_names_language ON public.notification_localizations(name,language);

-- a view to determine the activity of mentors
CREATE OR REPLACE VIEW public.active_mentors AS
SELECT activity_date AS date, COUNT(*) AS count FROM
(
    SELECT DISTINCT * FROM
    (
        SELECT CAST(login_log_entries.log_time AS DATE) AS activity_date, login_log_entries.user_id
        FROM login_log_entries
                 JOIN users ON users.Id = login_log_entries.user_id
        WHERE users.role = 3 OR users.role = 4
        GROUP BY CAST(login_log_entries.log_time AS DATE), login_log_entries.user_id

        UNION

        SELECT CAST(chat_messages.sender_date AS DATE) AS activity_date, chat_messages.sender_id AS user_id
        FROM chat_messages
        JOIN users ON users.Id = chat_messages.sender_id
        WHERE users.role = 3 OR users.role = 4
        GROUP BY CAST(chat_messages.sender_date AS DATE), chat_messages.sender_id

        UNION

        SELECT CAST(subscription_date AS DATE) AS activity_date, id FROM users
        WHERE users.role = 3 OR users.role = 4

        UNION

        SELECT CAST(review_date AS DATE), mentor_id AS user_id
        FROM lesson_submissions
        JOIN Enrollments ON Enrollments.Id = lesson_submissions.enrollment_id
        GROUP BY CAST(review_date AS DATE), mentor_id

    ) AS B
) AS A
GROUP BY A.activity_date;

-- a view to determine the activity of students
CREATE OR REPLACE VIEW active_students AS
SELECT activity_date AS date, COUNT(*) AS count FROM
(
    SELECT DISTINCT * FROM
    (
        SELECT CAST(login_log_entries.log_time AS DATE) AS activity_date, login_log_entries.user_id
        FROM login_log_entries
                 JOIN users ON users.Id = login_log_entries.user_id
        WHERE users.role = 1 OR users.role = 2
        GROUP BY CAST(login_log_entries.log_time AS DATE), login_log_entries.user_id

        UNION

        SELECT CAST(chat_messages.sender_date AS DATE) AS activity_date, chat_messages.sender_id AS user_id
        FROM chat_messages
                 JOIN users ON users.Id = chat_messages.sender_id
        WHERE users.role = 1 OR users.role = 2
        GROUP BY CAST(chat_messages.sender_date AS DATE), chat_messages.sender_id

        UNION

        SELECT CAST(subscription_date AS DATE) AS activity_date, id FROM users
        WHERE users.role = 1 OR users.role = 2

    ) AS B
) AS A
GROUP BY A.activity_date;

-- this view will determine the number of active students for each mentor
CREATE OR REPLACE VIEW active_students_per_mentor AS
SELECT g.mentor_id, COUNT(g.student_id) AS active_students FROM
(
    SELECT j.mentor_id, j.student_id FROM
    (
        SELECT DISTINCT e.mentor_id, e.student_id, e.closure_date, s.student_active_days
        FROM enrollments AS e, settings AS s
    ) AS j
    JOIN user_last_activity AS a ON a.user_id = j.student_id
    WHERE a.activity_date >= clock_timestamp() + interval '1 day' * j.student_active_days
    AND j.closure_date IS NULL AND mentor_id IS NOT NULL
) AS g
GROUP BY mentor_id;

-- a view to deal with content pages and their overrides
CREATE OR REPLACE VIEW content_page_overrides AS
SELECT
    c.id, s.id AS specific_content_id, s.creation_date, s.modification_date,
    c.Type, c.sequence_id, s.is_published, c.name, c.permalink, s.content,
    FALSE AS is_html_content, s.site
FROM site_specific_content_pages AS s
JOIN content_pages AS c ON s.content_page_id = c.id;

-- a view that estimates the reading time of a course
CREATE OR REPLACE VIEW course_reading_time AS
SELECT c.id AS course_id, rt.revision_id, rt.reading_time, lc.count AS lesson_count, rt.reading_time / lc.count AS average_reading_time
FROM
(
    SELECT rt.revision_id, SUM(reading_time) AS reading_time
    FROM
    (
        SELECT c.id AS revision_id, LENGTH(LT.Content) / 750.0  AS reading_time
        FROM lesson_text_elements AS LT
        JOIN lessons AS l ON l.id = lt.lesson_id
        JOIN course_revisions AS c on c.id = l.course_revision_id
        WHERE c.stage = 3
    ) AS rt
    GROUP BY rt.revision_id
) AS rt
JOIN course_revisions AS cr on cr.id = rt.revision_id
JOIN courses AS c ON c.Id = cr.course_id
JOIN
(
    SELECT course_revision_id, COUNT(id) AS count FROM lessons
    GROUP BY course_revision_id
) AS lc
ON lc.course_revision_id = cr.id;

-- a view with for each course the number of students
CREATE OR REPLACE VIEW course_student_count AS
SELECT id AS course_id, CASE WHEN ec.count IS NULL THEN 0 ELSE ec.count END AS count
FROM courses
LEFT JOIN
(
    SELECT COUNT(cr.course_id) AS count, cr.course_id AS course_id
    FROM enrollments
    JOIN course_revisions cr on cr.id = enrollments.course_revision_id
    GROUP BY cr.course_id
) AS ec ON ec.course_id = Id;

-- A view which determine which lessons is next
CREATE OR REPLACE VIEW enrollment_next_lessons AS
SELECT n.enrollment_id, n.lesson_id FROM
(
    SELECT enrollment_id, lesson_id, ROW_NUMBER() OVER(PARTITION BY enrollment_id ORDER BY sequence_id) AS Rank
    FROM
    (
        SELECT e.id AS enrollment_id,  l.id AS lesson_id, l.sequence_id
        FROM enrollments AS e
        JOIN course_revisions AS cr ON cr.id = e.course_revision_id
        JOIN lessons AS l ON  CR.Id = l.course_revision_id
    ) AS a
    WHERE lesson_id NOT IN
    (
        SELECT lesson_id
        FROM lesson_submissions
        WHERE is_final = TRUE AND enrollment_id = a.enrollment_id
    )
) AS n
WHERE n.rank = 1;

-- a view with information about enrollments
CREATE OR REPLACE VIEW enrollment_summaries AS
SELECT
    a.id, a.creation_date, a.student_id, a.mentor_id, s.name AS student_name,
    a.course_revision_id, m.name AS mentor_name, c.name AS course_name,
    ls.submitted, l.lesson_count, n.lesson_id as next_lesson_id, ln.name AS next_lesson_name,
    st.activity_date as student_last_active, cr.course_id,
    CASE WHEN (a.closure_date IS NULL OR a.closure_date > clock_timestamp()) THEN TRUE ELSE FALSE END AS is_active,
    a.is_course_completed, a.allow_retake, Rank, a.closure_date
FROM
(
    SELECT *, ROW_NUMBER() OVER(PARTITION BY student_id ORDER BY creation_date DESC) AS Rank
    FROM enrollments
) AS a
JOIN users AS s ON a.student_id = s.id
JOIN users AS m ON a.mentor_id = m.id
JOIN course_revisions AS cr ON cr.id = a.course_revision_id
JOIN courses AS c ON c.Id = cr.course_id
LEFT JOIN
(
    SELECT Count(id) AS submitted, enrollment_id
    FROM lesson_submissions
    WHERE is_final = TRUE
    GROUP BY enrollment_id
)
AS ls ON ls.enrollment_id = a.id
JOIN
(
    SELECT course_revision_id, COUNT(Id) as lesson_count
    FROM lessons
    GROUP BY course_revision_id
)
AS l ON l.course_revision_id = a.course_revision_id
LEFT JOIN enrollment_next_lessons as n ON a.Id = n.enrollment_id
LEFT JOIN lessons AS ln ON ln.Id = n.lesson_id
JOIN user_last_activity AS st ON st.user_id = a.student_id;

-- A view with the view it took to review a lesson
CREATE OR REPLACE VIEW lesson_review_time AS
SELECT
    l.enrollment_id,
    l.submission_date,
    EXTRACT(EPOCH FROM (submission_date - review_date)) / 86400.0 as review_time
FROM lesson_submissions AS l
JOIN enrollments AS e ON l.enrollment_id = e.id
WHERE l.is_final = TRUE AND e.mentor_id IS NOT NULL AND l.submission_date IS NOT NULL AND l.review_date IS NOT NULL;

-- create a view with mentor performance --
CREATE OR REPLACE VIEW mentor_performance AS
SELECT average_messages.mentor_id, average_message_per_student, review_time_span, days_active, rating.average_rating
FROM
(
    SELECT sender_id AS mentor_id, AVG(message_count) AS average_message_per_student
    FROM
    (
        SELECT sender_id, COUNT(id) AS message_count
        FROM chat_messages
        GROUP BY sender_id, channel_id
    ) AS messages
    GROUP BY sender_id
) AS average_messages
JOIN
(
    SELECT
        AVG(EXTRACT(EPOCH FROM (submission_date - review_date))) AS review_time_span,
        enrollments.mentor_id
    FROM lesson_submissions
    JOIN enrollments ON enrollments.id = lesson_submissions.enrollment_id
    WHERE is_final = TRUE AND review_date IS NOT NULL AND submission_date IS NOT NULL
    GROUP BY enrollments.mentor_id
) AS review_performance
ON review_performance.mentor_id = average_messages.mentor_id
LEFT JOIN
(
    SELECT COUNT(date) AS days_active, user_id AS mentor_id
    FROM
    (
        SELECT DISTINCT user_id, date FROM
        (
            SELECT DISTINCT sender_id AS user_id, CAST(sender_date AS DATE) AS date
            FROM chat_messages
            WHERE sender_date > (clock_timestamp() - INTERVAL '1 year')
            GROUP BY sender_id,  CAST(sender_date AS DATE)
            UNION
            SELECT DISTINCT user_id, CAST(log_time AS DATE) AS date
            FROM login_log_entries
            WHERE log_time > (clock_timestamp() - INTERVAL '1 year')
            UNION
            SELECT DISTINCT mentor_id AS user_id, CAST(review_date AS DATE)
            FROM lesson_submissions
            JOIN enrollments ON enrollments.id = lesson_submissions.enrollment_id
            WHERE review_date > (clock_timestamp() - INTERVAL '1 year')
        )
        AS activities
    ) AS D
    GROUP BY user_id
) AS activity
ON activity.mentor_id = average_messages.mentor_id
LEFT JOIN
(
    SELECT AVG(CAST(rating as Float)) AS average_rating, mentor_id
    FROM mentor_ratings
    GROUP BY mentor_id
) AS rating
ON rating.mentor_id = average_messages.mentor_id