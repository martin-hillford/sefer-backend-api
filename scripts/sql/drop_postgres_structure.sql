DROP VIEW public.active_mentors CASCADE;
DROP VIEW public.active_students CASCADE;
DROP VIEW public.active_students_per_mentor CASCADE;
DROP VIEW public.content_page_overrides CASCADE;
DROP VIEW public.course_reading_time CASCADE;
DROP VIEW public.course_student_count CASCADE;
DROP VIEW public.enrollment_next_lessons CASCADE;
DROP VIEW public.lesson_review_time CASCADE;
DROP TABLE public.curriculum_block_courses;
DROP TABLE public.curriculum_blocks;
DROP TABLE public.curriculum_revisions;
DROP TABLE public.ip_address_lookups;
DROP TABLE public.curricula;
DROP TABLE public.course_prerequisites;
DROP TABLE public.site_specific_content_pages CASCADE ;
DROP TABLE public.content_pages;
DROP TABLE public.client_page_request_log_entries;
DROP TABLE public.chat_channel_receivers;
DROP TABLE public.chat_channel_messages;
DROP TRIGGER tg_chat_messages_update ON public.chat_messages;
DROP FUNCTION fn_tg_chat_messages_insert;
DROP TABLE public.chat_messages CASCADE;
DROP TABLE public.chat_channels;
DROP TABLE public.blogs;
DROP TABLE public.api_request_Log_entries;
DROP TABLE public.answers;
DROP TRIGGER tg_lesson_submissions_mentor_update ON public.lesson_submissions;
DROP FUNCTION fn_tg_lesson_submissions_mentor_update;
DROP TRIGGER tg_lesson_submissions_student_update ON public.lesson_submissions;
DROP FUNCTION fn_tg_lesson_submissions_student_update;
DROP TRIGGER tg_lesson_submissions_insert ON public.lesson_submissions;
DROP FUNCTION fn_tg_lesson_submissions_insert;
DROP TABLE public.lesson_submissions CASCADE;
DROP TABLE public.__Migrations;
DROP TABLE public.lesson_text_elements;
DROP TABLE public.lesson_open_questions;
DROP TABLE public.lesson_multiple_choice_question_choices;
DROP TABLE public.lesson_multiple_choice_questions;
DROP TABLE public.lesson_media_elements;
DROP TABLE public.lesson_bool_questions;
DROP TABLE public.lessons;
DROP TABLE public.admin_settings;
DROP TABLE public.testimonies;
DROP TABLE public.survey_results;
DROP TABLE public.course_ratings;
DROP TABLE public.mentor_ratings;
DROP TABLE public.reward_enrollments;
DROP TRIGGER tg_enrollments_insert ON public.enrollments;
DROP FUNCTION fn_tg_enrollments_insert;
DROP TABLE public.enrollments;
DROP TABLE public.mentor_regions;
DROP TABLE public.reward_grants;
DROP TABLE public.mentor_settings;
DROP TABLE public.mentor_courses;
DROP TABLE public.mentor_student_data;
DROP TABLE public.user_backup_keys;
DROP TABLE public.survey_open_questions;
DROP TABLE public.survey_multiple_choice_question_choices;
DROP TABLE public.survey_multiple_choice_questions;
DROP TABLE public.survey_bool_questions;
ALTER TABLE public.course_revisions DROP CONSTRAINT fk_course_revisions_survey_id;
DROP TABLE public.surveys;
DROP TABLE public.course_revisions;
DROP TABLE public.settings;
DROP TABLE public.series_courses;
DROP TABLE public.courses;
DROP TABLE public.series;
DROP TABLE public.reward_targets;
DROP TABLE public.rewards;
DROP TABLE public.logs;
DROP TABLE public.short_urls;
DROP TABLE public.student_settings;
DROP TRIGGER tg_login_log_entries_insert ON public.login_log_entries;
DROP FUNCTION fn_tg_login_log_entries_insert;
DROP TABLE public.login_log_entries;
DROP TABLE public.push_notification_tokens;
DROP TABLE public.user_last_activity;
DROP TRIGGER tg_users_insert ON public.users;
DROP FUNCTION fn_tg_users_insert;
DROP TABLE public.users;
DROP TABLE public.donations;
DROP TABLE public.templates;